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
    public class sdshippack
    {
        public sdshippack()
        {
			
        }
 
       #region  sdshippack实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packlistno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime packdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packuom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int defaultuomqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stuffingqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool stuffinginfull {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentbundleno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packweekno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vehicleno {get; set;}
	
	
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
	   public string packcode {get; set;}
	
	
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
 
 