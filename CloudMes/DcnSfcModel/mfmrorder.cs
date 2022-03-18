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
    public class mfmrorder
    {
        public mfmrorder()
        {
			
        }
 
       #region  mfmrorder实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mfmrorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mfmrtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime requestdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tofactoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string requestby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lastprocessno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool partial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool completed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime completedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancelled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mrnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string batchno {get; set;}
	
	
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
	   public string field3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scmorderno {get; set;}
	
	
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
 
 