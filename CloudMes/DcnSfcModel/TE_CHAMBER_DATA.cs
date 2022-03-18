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
    public class TE_CHAMBER_DATA
    {
        public TE_CHAMBER_DATA()
        {
			
        }
 
       #region  TE_CHAMBER_DATA实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHAMBER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOCATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UUTTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UUTNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COUNTS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime STIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TESTTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ETIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RECORDTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Information {get; set;}
	
	   
	   #endregion
    }
}
 
 