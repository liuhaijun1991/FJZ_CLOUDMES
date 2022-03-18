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
    public class mfmpsplanhist
    {
        public mfmpsplanhist()
        {
			
        }
 
       #region  mfmpsplanhist实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime upddatedt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string planname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsweekofmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsday {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsdayofweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lastqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal planqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal executeqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal actualqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool confirmed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string confirmby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime confirmdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool converted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string convertby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime convertdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool dayoff {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool workondayoff {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note2 {get; set;}
	
	
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
 
 