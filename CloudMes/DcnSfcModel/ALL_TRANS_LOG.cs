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
    public class ALL_TRANS_LOG
    {
        public ALL_TRANS_LOG()
        {
			
        }
 
       #region  ALL_TRANS_LOG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BATCHID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MODULEID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MODULENAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTIONID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MSG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime STARTTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ENDTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int TIMEDIFF {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int AFFECTEDCOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CREATEBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATEDT {get; set;}
	
	   
	   #endregion
    }
}
 
 