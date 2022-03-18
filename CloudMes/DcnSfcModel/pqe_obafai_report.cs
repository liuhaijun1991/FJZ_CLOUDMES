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
    public class pqe_obafai_report
    {
        public pqe_obafai_report()
        {
			
        }
 
       #region  pqe_obafai_report实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string serialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FAIoccurdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAItype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodserial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ECO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime planshipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string inFAIresult {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custapproved {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime custapproveddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string issuedby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reporttype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string addcontent {get; set;}
	
	
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
 
 