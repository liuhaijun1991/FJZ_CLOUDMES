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
    public class mfctbmrp
    {
        public mfctbmrp()
        {
			
        }
 
       #region  mfctbmrp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ctbdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string etddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal mrppoqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal qtyunit {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qtybase {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string qtyuom {get; set;}
	
	   
	   #endregion
    }
}
 
 