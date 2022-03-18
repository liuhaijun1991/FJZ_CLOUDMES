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
    public class eSeqNo
    {
        public eSeqNo()
        {
			
        }
 
       #region  eSeqNo实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SeqNoName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SeqNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Digits {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Minimum {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Maximum {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Prefixed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SeqNoForm {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SeqTableName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UseDate {get; set;}
	
	   
	   #endregion
    }
}
 
 