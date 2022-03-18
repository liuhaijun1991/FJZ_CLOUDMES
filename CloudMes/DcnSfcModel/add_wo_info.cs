using SqlSugar;
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
    public class add_wo_info
    {
        public add_wo_info()
        {
			
        }

        #region  add_wo_info实体



        [SugarColumn(IsPrimaryKey = true)]
        public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool packed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool completed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool shipped {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool controlflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carton_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string location {get; set;}
	
	
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
 
 