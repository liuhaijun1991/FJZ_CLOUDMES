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
    public class OEORDLIN
    {
        public OEORDLIN()
        {
			
        }
 
       #region  OEORDLIN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SEQUENCE_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILLER_0001 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOCATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PICK_SEQ {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CUST_ITEM_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_ORDERED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_TO_SHIP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UNIT_PRICE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DISCOUNT_PCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal REQUEST_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_BACK_ORDERED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_RETURN_TO_STOCK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BACKORDERABLE_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UNIT_OF_MEASURE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UOM_RATIO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UNIT_COST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UNIT_WEIGHT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMM_CALC_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMM_PCT_OR_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PROMISE_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAXABLE_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STOCKED_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLLED_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SELECT_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_QTY_ORDERED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_QTY_SHIPPED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAXABLE_FLAG_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAXABLE_FLAG_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAXABLE_FLAG_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRICE_ORG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COPY_TO_BM_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EXPLODE_KIT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal BM_ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal DATE_TO_ALLOCATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal LAST_POST_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string POST_TO_INV_QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string POSTED_TO_INV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOT_QTY_POSTED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_ALLOCATED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMPONENTS_ALLOC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BIN_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COST_METHOD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SER_LOT_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MULT_FTR_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LINE_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_PROD_CAT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITM_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REASON_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FEATURE_RETURN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REC_INSPECTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_FROM_STOCK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MULTI_RELEASE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal REQ_SHIP_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_FROM_STOCK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_5 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PICK_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal SHIP_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal BILL_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UPDATE_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRICE_CODE_ORG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_SCHEDULE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ITEM_CUSTOMER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QTY_BO_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal UNIQUE_SEQ {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MFG_METHOD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FORCED_DEMAND {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal CONF_PICK_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ITEM_RELEASE_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BIN_SER_LOT_COMP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OFFSET_USED_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ECS_SPACE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_ORDER_STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_COST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_ORD_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal RMA_LINE_SEQ_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VEND_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILLER_0004 {get; set;}
	
	   
	   #endregion
    }
}
 
 