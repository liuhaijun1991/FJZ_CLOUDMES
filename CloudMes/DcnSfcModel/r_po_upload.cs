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
    public class r_po_upload
    {
        public r_po_upload()
        {
			
        }
 
       #region  r_po_upload实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string po_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int po_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ext_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string uploadby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddt {get; set;}
	
	   
	   #endregion
    }
}
 
 