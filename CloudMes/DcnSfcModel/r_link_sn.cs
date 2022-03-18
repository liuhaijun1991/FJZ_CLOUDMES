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
    public class r_link_sn
    {
        public r_link_sn()
        {
			
        }
 
       #region  r_link_sn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LinkCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tr_code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LinkQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ScanQty {get; set;}
	
	
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
 
 