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
    public class STO_PO_Header
    {
        public STO_PO_Header()
        {
			
        }
 
       #region  STO_PO_Header实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sotype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int convertflag {get; set;}
	
	   
	   #endregion
    }
}
 
 