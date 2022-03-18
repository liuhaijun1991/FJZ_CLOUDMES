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
    public class mfworkstatus_temp2
    {
        public mfworkstatus_temp2()
        {
			
        }
 
       #region  mfworkstatus_temp2实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime assigndate {get; set;}
	
	
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
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int buildno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool started {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool packed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime packdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool completed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime completedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool repairheld {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime repairdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentpdline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentshift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double stuffingqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Quited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime QuitDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEvent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ReseatCount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Reseat {get; set;}
	
	
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
	   public bool ReFlow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReFlowTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ORT_IN_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ORT_OUT_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ORT_FAIL_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ORT_COUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ORT_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ORT_OUTFLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stockstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime stockintime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime stockouttime {get; set;}
	
	   
	   #endregion
    }
}
 
 