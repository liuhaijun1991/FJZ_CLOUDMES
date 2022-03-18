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
    public class sfcrepairpart_temp
    {
        public sfcrepairpart_temp()
        {
			
        }
 
       #region  sfcrepairpart_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ActScanDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurepartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failurepartsn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string newpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string newpartsn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ooinv {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ooinvdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mrorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
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
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mdsget {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Debug {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failMPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string newMPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scan_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandt {get; set;}
	
	   
	   #endregion
    }
}
 
 