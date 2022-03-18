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
    public class mfworkorder_RMAImport
    {
        public mfworkorder_RMAImport()
        {
			
        }
 
       #region  mfworkorder_RMAImport实体
 
	
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
	   public DateTime workorderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scheduledate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkRouteType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrderType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuversion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skudesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custmodelno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customerid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string software {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string firmware {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eeecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productfamily {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productlevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productcolor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productlangulage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prioritycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcountry {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string compcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool released {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime releaseddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobstarted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mrpartial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mrcompleted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancelled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime closedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int workorderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int finishedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ScrapedQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stockinrequestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stockinprocessqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string batchno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime batchdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int batchseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string jobnote1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RMANo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RMALineNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Reworked {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartversion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OrderLineNo {get; set;}
	
	
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
	   public bool Alert {get; set;}
	
	   
	   #endregion
    }
}
 
 