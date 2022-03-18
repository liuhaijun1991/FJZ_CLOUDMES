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
    public class mfctbdailypart
    {
        public mfctbdailypart()
        {
			
        }
 
       #region  mfctbdailypart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bomname {get; set;}
	
	
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
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
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
	   public decimal totalpoqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totaldnqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etddate {get; set;}
	
	
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
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentpartno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentpartno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentpartno3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacepartno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacepartno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacepartno3 {get; set;}
	
	   
	   #endregion
    }
}
 
 