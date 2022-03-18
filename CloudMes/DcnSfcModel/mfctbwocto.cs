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
    public class mfctbwocto
    {
        public mfctbwocto()
        {
			
        }
 
       #region  mfctbwocto实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime workorderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scheduledate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool enableproduct {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string revno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal orderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal jobqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal commitqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string oldrevno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal oldorderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal oldjobqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal oldshipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal oldcommitqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string oldfilename {get; set;}
	
	
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
 
 