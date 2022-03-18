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
    public class scmvendorhubinv
    {
        public scmvendorhubinv()
        {
			
        }
 
       #region  scmvendorhubinv实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string warehouseid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lowqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal safetyqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal highqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal currentqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal onwayqty {get; set;}
	
	
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
	   public decimal inhubqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal inmfqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal reserveqty {get; set;}
	
	   
	   #endregion
    }
}
 
 