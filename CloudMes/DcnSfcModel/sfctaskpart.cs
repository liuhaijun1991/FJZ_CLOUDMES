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
    public class sfctaskpart
    {
        public sfctaskpart()
        {
			
        }
 
       #region  sfctaskpart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short buildseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int replaceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacetopartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal qty {get; set;}
	
	
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
 
 