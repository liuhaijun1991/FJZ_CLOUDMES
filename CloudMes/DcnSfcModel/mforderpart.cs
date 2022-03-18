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
    public class mforderpart
    {
        public mforderpart()
        {
			
        }
 
       #region  mforderpart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ordertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime orderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string todate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double stockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double buildqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double bohqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double arrqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double ctbqty {get; set;}
	
	
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
 
 