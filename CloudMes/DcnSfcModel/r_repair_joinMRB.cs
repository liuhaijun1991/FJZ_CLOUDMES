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
    public class r_repair_joinMRB
    {
        public r_repair_joinMRB()
        {
			
        }
 
       #region  r_repair_joinMRB实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productfamily {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurestation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurecategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool repaired {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string restorefailurestation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partcategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime executeDt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string from_storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string to_storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reason_code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mrbby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime mrbtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime executeDt2 {get; set;}
	
	   
	   #endregion
    }
}
 
 