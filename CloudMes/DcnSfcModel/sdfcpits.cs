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
    public class sdfcpits
    {
        public sdfcpits()
        {
			
        }
 
       #region  sdfcpits实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string todate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromday {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string today {get; set;}
	
	
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
 
 