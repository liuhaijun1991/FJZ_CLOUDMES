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
    public class sfcSPCReportModel
    {
        public sfcSPCReportModel()
        {
			
        }
 
       #region  sfcSPCReportModel实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string hourperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int wrongpartsqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int wrongdotsqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int boardstotal {get; set;}
	
	
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
	   public bool corrected {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string correctby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime correctdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int rowid {get; set;}
	
	   
	   #endregion
    }
}
 
 