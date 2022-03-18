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
    public class sfcoba
    {
        public sfcoba()
        {
			
        }
 
       #region  sfcoba实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AQLType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lotqtymin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lotqtymax {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int rejectedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string flag {get; set;}
	
	
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
 
 