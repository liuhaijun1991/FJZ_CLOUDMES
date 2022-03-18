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
    public class sfcSimpleTestConfig
    {
        public sfcSimpleTestConfig()
        {
			
        }
 
       #region  sfcSimpleTestConfig实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lotqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GIlevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sampleqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int acceptqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int rejectqty {get; set;}
	
	   
	   #endregion
    }
}
 
 