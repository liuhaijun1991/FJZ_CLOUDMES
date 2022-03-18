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
    public class mfwipreport_Qty
    {
        public mfwipreport_Qty()
        {
			
        }
 
       #region  mfwipreport_Qty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Category {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Hold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CB {get; set;}
	
	   
	   #endregion
    }
}
 
 