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
    public class nsdirepairwip
    {
        public nsdirepairwip()
        {
			
        }
 
       #region  nsdirepairwip实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CodeName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PCBA_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PCBA_WO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FailDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckinDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureSympton {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Area {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WipStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FailTimeAging {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CheckinTimeAging {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkTime {get; set;}
	
	   
	   #endregion
    }
}
 
 