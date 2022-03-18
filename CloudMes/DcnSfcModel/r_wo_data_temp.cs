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
    public class r_wo_data_temp
    {
        public r_wo_data_temp()
        {
			
        }
 
       #region  r_wo_data_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime datetime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SIPQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FGIQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QFGIQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int OPENWOQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int failqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string wo_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int woqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ROPshorting {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int r_value {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int suggestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobdone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 