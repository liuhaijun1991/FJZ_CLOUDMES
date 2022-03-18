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
    public class mfworkorderwip_3
    {
        public mfworkorderwip_3()
        {
			
        }
 
       #region  mfworkorderwip_3实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short eventseqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qty {get; set;}
	
	   
	   #endregion
    }
}
 
 