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
    public class printlabeldetail
    {
        public printlabeldetail()
        {
			
        }
 
       #region  printlabeldetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VERSION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Label_pn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string new_pn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal workorderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal instockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal stockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal waitprintqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal waittosendqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal outstockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal checkoutqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkouttime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkoutby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int reprintqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reprint_rate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime reprinttime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reprintby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string remark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalstockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string totalinfo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime deliverytime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string data1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int data2 {get; set;}
	
	
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
 
 