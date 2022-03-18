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
    public class pqe_let_report
    {
        public pqe_let_report()
        {
			
        }
 
       #region  pqe_let_report实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lotno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lettype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string client {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string catena {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lapsedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fliename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte[] data1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string data2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime data3 {get; set;}
	
	   
	   #endregion
    }
}
 
 