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
    public class mfeeelookup
    {
        public mfeeelookup()
        {
			
        }
 
       #region  mfeeelookup实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string briefcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productgroup {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short startdigit {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short countdigit {get; set;}
	
	
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
 
 