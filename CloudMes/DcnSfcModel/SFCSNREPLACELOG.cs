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
    public class SFCSNREPLACELOG
    {
        public SFCSNREPLACELOG()
        {
			
        }
 
       #region  SFCSNREPLACELOG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LOGDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BOXSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OLDSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NEWSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EVENTPOINT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EVENTDETAIL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EVENTBY {get; set;}
	
	   
	   #endregion
    }
}
 
 