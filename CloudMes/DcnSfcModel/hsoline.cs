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
    public class hsoline
    {
        public hsoline()
        {
			
        }
 
       #region  hsoline实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sono {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string solineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tpcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rectype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string itemcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string itemdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string uom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal orderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal cancelqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal balanceqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string onhold {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal price {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime escdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eccno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string htsno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string freiterms {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custfreiacct {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime promiseddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scheduleddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string wharehouse {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoloc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptocustname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoadd1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoadd2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoadd3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoadd4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptocity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptostate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptozip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptoprovince {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiptocountry {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contact {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string linecomments {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string btoindicator {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bundlepartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bundlepartdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal bundlepprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal bundlesprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LEC_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ShippAble {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Reference {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bundlelineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field5 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field6 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field7 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field8 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field9 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field10 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field11 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field12 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field13 {get; set;}
	
	   
	   #endregion
    }
}
 
 