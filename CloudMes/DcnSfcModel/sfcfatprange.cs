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
    public class sfcfatprange
    {
        public sfcfatprange()
        {
			
        }
 
       #region  sfcfatprange实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string modelno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sortno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string audittype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lotminqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lotmaxqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sampleqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skumodel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
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
 
 