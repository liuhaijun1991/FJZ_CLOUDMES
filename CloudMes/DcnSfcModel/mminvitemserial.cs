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
    public class mminvitemserial
    {
        public mminvitemserial()
        {
			
        }
 
       #region  mminvitemserial实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string receiveno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime receivedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdt {get; set;}
	
	   
	   #endregion
    }
}
 
 