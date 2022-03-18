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
    public class R_Trigger_EDI
    {
        public R_Trigger_EDI()
        {
			
        }
 
       #region  R_Trigger_EDI实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TriggerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Supplier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReleaseDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Finishedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Openqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Needdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CommitDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ResponseDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Manual {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ChangeReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BrdComment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SupComment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 