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
    public class sfcrepairmain
    {
        public sfcrepairmain()
        {
			
        }
 
       #region  sfcrepairmain实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productfamily {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failuredate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failuretime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fworkdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fworktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurepdline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failureshift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failureeventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurestation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurecategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool repaired {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rworkdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rworktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairpdline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairshift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairstation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string actualfailurecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reworksolutioncode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string restorefailurestation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reworkremark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partcategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool failcodedload {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool failcodedled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool repairing {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool badever {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool undetermined {get; set;}
	
	
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
	   public DateTime FinishDate {get; set;}
	
	   
	   #endregion
    }
}
 
 