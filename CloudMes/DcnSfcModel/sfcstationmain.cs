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
    public class sfcstationmain
    {
        public sfcstationmain()
        {
			
        }
 
       #region  sfcstationmain实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stationtask {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string macaddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stationdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stationtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool nomasterserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool collectevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool collectcserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool countyieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobstartpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobfinishpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkkeypart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printtravelcard {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printseriallabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printenetid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printfglabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printshiplabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printpackagelabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printpacklistlabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool printwilabel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelprinterport {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelprintertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string formprinterport {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string formprintertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reportprinterport {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reportprintertype {get; set;}
	
	
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
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 