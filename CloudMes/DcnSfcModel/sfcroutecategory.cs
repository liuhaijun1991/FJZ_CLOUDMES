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
    public class sfcroutecategory
    {
        public sfcroutecategory()
        {
			
        }
 
       #region  sfcroutecategory实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int eventseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodcategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int scanlength {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scantype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ScaleFactor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool optioncategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int unitofscan {get; set;}
	
	   
	   #endregion
    }
}
 
 