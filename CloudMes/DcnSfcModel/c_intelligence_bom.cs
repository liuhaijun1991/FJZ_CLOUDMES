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
    public class c_intelligence_bom
    {
        public c_intelligence_bom()
        {
			
        }
 
       #region  c_intelligence_bom实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Item {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rev {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ItemDesc {get; set;}
	
	
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
 
 