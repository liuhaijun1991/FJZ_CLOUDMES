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
    public class mfskunogroupid
    {
        public mfskunogroupid()
        {
			
        }
 
       #region  mfskunogroupid实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Type_IU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HSCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string groupid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal price {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal netweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte[] registrationno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long dw_id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 