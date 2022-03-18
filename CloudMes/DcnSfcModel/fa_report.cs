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
    public class fa_report
    {
        public fa_report()
        {
			
        }
 
       #region  fa_report实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string years {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string week {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime rmarequestdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string supplier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string modul {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string casedesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pqedesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string se {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string implementstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string attachment1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sqe {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string brocwindow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sqefeedback {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string attachment2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pqe {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string overdue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime editdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime closedt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte step {get; set;}
	
	   
	   #endregion
    }
}
 
 