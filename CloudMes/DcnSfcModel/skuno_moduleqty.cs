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
    public class skuno_moduleqty
    {
        public skuno_moduleqty()
        {
			
        }
 
       #region  skuno_moduleqty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctltype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno_share {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short moduleqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte SMT_allpart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte PTH_allpart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool check_SKU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short length {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prexid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctlcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string snvalue {get; set;}
	
	
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
 
 