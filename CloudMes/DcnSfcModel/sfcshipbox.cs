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
    public class sfcshipbox
    {
        public sfcshipbox()
        {
			
        }
 
       #region  sfcshipbox实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime boxdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string packno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int boxseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sscccode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal uweight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string trackingno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string zone {get; set;}
	
	
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
 
 