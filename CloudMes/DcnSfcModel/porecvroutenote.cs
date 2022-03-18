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
    public class porecvroutenote
    {
        public porecvroutenote()
        {
			
        }
 
       #region  porecvroutenote实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string podnno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routepoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime notedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 