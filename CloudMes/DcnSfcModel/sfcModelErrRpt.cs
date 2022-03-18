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
    public class sfcModelErrRpt
    {
        public sfcModelErrRpt()
        {
			
        }
 
       #region  sfcModelErrRpt实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Modelno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short PartCount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short dotCount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short targetdotalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short lowdot {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short hidotalert {get; set;}
	
	   
	   #endregion
    }
}
 
 