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
    public class sfclotstatus
    {
        public sfclotstatus()
        {
			
        }
 
       #region  sfclotstatus实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lotno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AQLType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lotqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sampleqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int failqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int rejectqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool undetermined {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool running {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool completed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool rework {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool rejected {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool reworkstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location5 {get; set;}
	
	
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
 
 