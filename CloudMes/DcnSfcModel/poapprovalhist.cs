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
    public class poapprovalhist
    {
        public poapprovalhist()
        {
			
        }
 
       #region  poapprovalhist实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime histdate {get; set;}
	
	
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
	   public string approverorder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string userid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string title {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string username {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime actiondate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool approved {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool reject {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rejectreason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lupeditby {get; set;}
	
	   
	   #endregion
    }
}
 
 