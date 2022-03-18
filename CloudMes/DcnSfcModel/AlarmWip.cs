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
    public class AlarmWip
    {
        public AlarmWip()
        {
			
        }
 
       #region  AlarmWip实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alarm_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Runintype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Esstype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vskuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipQty2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shippedQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CBSQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipqty_gap {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipqty_status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FQADate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQADateStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQAQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQAQty_temp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double FQAyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQAQty_gap {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQAstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FQAQty_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RuninDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninDateStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninWipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninTestQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninQty_Realtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double Runinyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninQty_gap {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Runinstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninQty_temp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninQty_temp1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RuninQty_temp2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EssDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssDateStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssWipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssTestQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssQty_Realtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double Essyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssQty_gap {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ESSstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssQty_temp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssQty_temp1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EssQty_temp2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime alarmDate {get; set;}
	
	   
	   #endregion
    }
}
 
 