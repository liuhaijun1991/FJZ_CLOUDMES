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
    public class sfctestcontrol
    {
        public sfctestcontrol()
        {
			
        }
 
       #region  sfctestcontrol实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string teststation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testcount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int controltimes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string controlflag {get; set;}
	
	
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
 
 