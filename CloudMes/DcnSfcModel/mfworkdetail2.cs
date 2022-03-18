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
    public class mfworkdetail2
    {
        public mfworkdetail2()
        {
			
        }
 
       #region  mfworkdetail2实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int replaceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacetopartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal issuedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal installedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal rtnrequestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal returnqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal scrapqty {get; set;}
	
	
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
	   public bool ismaterial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool consumptionitem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool keypart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool packmaterial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodcategoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal originalqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacegroup {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool noreplacepart {get; set;}
	
	
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
 
 