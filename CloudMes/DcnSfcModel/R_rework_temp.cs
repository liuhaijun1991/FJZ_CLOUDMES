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
    public class R_rework_temp
    {
        public R_rework_temp()
        {
			
        }
 
       #region  R_rework_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string stationcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string confirmed_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sap_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string storename {get; set;}
	
	
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
 
 