﻿using System;
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
    public class H_Receive_SN
    {
        public H_Receive_SN()
        {
			
        }
 
       #region  H_Receive_SN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TriggerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Supplier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPflag {get; set;}
	
	
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
 
 