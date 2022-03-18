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
    public class mfctbweek
    {
        public mfctbweek()
        {
			
        }
 
       #region  mfctbweek实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bomname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bomdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal sdqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal mpsqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal whqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal hubqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal boh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal arrival {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ctbqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal eoh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal mrpqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbmonthname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbweekofmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbweekname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ctbqtywh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ctbqtypo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ctbqtyhub {get; set;}
	
	
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
	   public decimal qtyunit {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qtybase {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string qtyuom {get; set;}
	
	   
	   #endregion
    }
}
 
 