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
    public class sfcyieldratedetail
    {
        public sfcyieldratedetail()
        {
			
        }
 
       #region  sfcyieldratedetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string postdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalfreshbuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalfreshpass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalfreshfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal freshyieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalreworkbuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalreworkpass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalreworkfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal reworkyieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalmsrbuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalmsrpass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalmsrfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal msryieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool spcfreshalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool spcreworkalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool spcmsralert {get; set;}
	
	
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
 
 