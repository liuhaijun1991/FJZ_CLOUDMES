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
    public class C_NETGEAR_PTM_CTL
    {
        public C_NETGEAR_PTM_CTL()
        {
			
        }
 
       #region  C_NETGEAR_PTM_CTL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TONO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ShipToPart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OrgCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ShipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PtmFile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool CQA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Converted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Sent {get; set;}
	
	
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
 
 