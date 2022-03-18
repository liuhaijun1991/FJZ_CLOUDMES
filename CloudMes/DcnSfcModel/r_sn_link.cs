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
    public class r_sn_link
    {
        public r_sn_link()
        {
			
        }
 
       #region  r_sn_link实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdt {get; set;}
	
	
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
 
 