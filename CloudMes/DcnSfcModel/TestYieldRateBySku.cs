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
    public class TestYieldRateBySku
    {
        public TestYieldRateBySku()
        {
			
        }
 
       #region  TestYieldRateBySku实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuserial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string wkfromto {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string yearwk {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testpassqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testfailqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testvlsqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int reqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lowest_fy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string related_Target {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double sigma3 {get; set;}
	
	
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
 
 