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
    public class r_repair_CheckInOut
    {
        public r_repair_CheckInOut()
        {
			
        }
 
       #region  r_repair_CheckInOut实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dataType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
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
	   public string codename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failstation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkinby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkindate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkout {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkoutdate {get; set;}
	
	
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
	   public DateTime executeDt {get; set;}
	
	   
	   #endregion
    }
}
 
 