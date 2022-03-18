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
    public class sfc_autoalarm_set
    {
        public sfc_autoalarm_set()
        {
			
        }
 
       #region  sfc_autoalarm_set实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AlertName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AlertDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AlertSql {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AlertMailID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AlertMailType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 