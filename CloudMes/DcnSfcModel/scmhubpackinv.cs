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
    public class scmhubpackinv
    {
        public scmhubpackinv()
        {
			
        }
 
       #region  scmhubpackinv实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmreceiveno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime receivedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmpalletno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal palletqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string areaid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipouted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipoutdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipoutrefno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipoutrefno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipoutrefno3 {get; set;}
	
	
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
 
 