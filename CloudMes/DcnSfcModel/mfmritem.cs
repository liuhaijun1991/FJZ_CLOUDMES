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
    public class mfmritem
    {
        public mfmritem()
        {
			
        }
 
       #region  mfmritem实体
 
	
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
	   public string mfmrpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromwhid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mrcompleted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal processqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
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
 
 