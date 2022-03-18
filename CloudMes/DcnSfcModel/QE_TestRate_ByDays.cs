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
    public class QE_TestRate_ByDays
    {
        public QE_TestRate_ByDays()
        {
			
        }
 
       #region  QE_TestRate_ByDays实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EVENTNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int PassQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FailQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int TotalQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime GetData {get; set;}
	
	   
	   #endregion
    }
}
 
 