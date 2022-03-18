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
    public class sfcfatpaudit
    {
        public sfcfatpaudit()
        {
			
        }
 
       #region  sfcfatpaudit实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime auditdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string audittype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LotQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LotBuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SubLotQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SubLotSeq {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool SubLotkey {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime resultdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string remark {get; set;}
	
	
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
 
 