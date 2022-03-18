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
    public class Panel_Sn_Temp
    {
        public Panel_Sn_Temp()
        {
			
        }
 
       #region  Panel_Sn_Temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string panel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 