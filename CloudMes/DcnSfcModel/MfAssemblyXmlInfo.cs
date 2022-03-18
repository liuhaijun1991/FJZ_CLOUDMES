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
    public class MfAssemblyXmlInfo
    {
        public MfAssemblyXmlInfo()
        {
			
        }
 
       #region  MfAssemblyXmlInfo实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ConfigHeaderID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BomConfigItemID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string XmlUrl {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CallUrl {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string XmlFile {get; set;}
	
	
	   /// <summary>
	   /// 0-wait_analyze
	   /// </summary>
	   public int Flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CreateTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EditTime {get; set;}
	
	   
	   #endregion
    }
}
 
 