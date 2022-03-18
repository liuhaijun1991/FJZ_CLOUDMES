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
    public class SFCB24F
    {
        public SFCB24F()
        {
			
        }
 
       #region  SFCB24F实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Palletno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Tqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Stock_Date {get; set;}
	
	   
	   #endregion
    }
}
 
 