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
    public class IQA
    {
        public IQA()
        {
			
        }
 
       #region  IQA实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string satus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string faildesc {get; set;}
	
	
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
 
 