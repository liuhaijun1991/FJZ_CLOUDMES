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
    public class msgreaderlog
    {
        public msgreaderlog()
        {
			
        }
 
       #region  msgreaderlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string referenceid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string messagetype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime readdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logdetail {get; set;}
	
	   
	   #endregion
    }
}
 
 