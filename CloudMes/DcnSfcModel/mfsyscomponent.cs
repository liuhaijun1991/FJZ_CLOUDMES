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
    public class mfsyscomponent
    {
        public mfsyscomponent()
        {
			
        }
 
       #region  mfsyscomponent实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int replaceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacetopartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool keypart {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool installed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal installedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eeecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodcategoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prodtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string originalqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replacegroup {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool noreplacepart {get; set;}
	
	
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
 
 