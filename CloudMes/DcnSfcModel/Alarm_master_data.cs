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
    public class Alarm_master_data
    {
        public Alarm_master_data()
        {
			
        }
 
       #region  Alarm_master_data实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vskuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string temp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double FQAuph {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double FQAyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double ESSyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double Runinyield {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double ESStime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double Runintime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double Packingtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double RunintoCBStime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string temp1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string temp2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 