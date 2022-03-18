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
    public class ie_sop_alarm
    {
        public ie_sop_alarm()
        {
			
        }
 
       #region  ie_sop_alarm实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string route {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pn_ver {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sop {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int quantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string wo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ps {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string send {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string receive {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string out_send {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string borrow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string in_receive {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string retur {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime acceptdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime outdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime indt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime returndt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime backdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string islook {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lookby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short alarm_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 