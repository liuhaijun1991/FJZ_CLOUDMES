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
    public class sdfcplan
    {
        public sdfcplan()
        {
			
        }
 
       #region  sdfcplan实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string planname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendaryear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string revision {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime revisedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string todate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal planqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal executeqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal actualqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool issued {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string issueby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string issuedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string issueweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note5 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note6 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note7 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note8 {get; set;}
	
	
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
 
 