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
    public class mfworkorderwip_2
    {
        public mfworkorderwip_2()
        {
			
        }
 
       #region  mfworkorderwip_2实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Series {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SubSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
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
	   public DateTime releasedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
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
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipment2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int pgi {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FailTotal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FailWIP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int checkin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int checkout {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int holdqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ortqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Critical {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Critical_Hold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REWIP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REWIP_Hold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REWIP_CB {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int InLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int InLine_Hold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int InLine_CB {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int TEMPWIP {get; set;}
	
	   
	   #endregion
    }
}
 
 