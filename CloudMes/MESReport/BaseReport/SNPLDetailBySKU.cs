using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNPLDetailBySKU : ReportBase
    {
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Staion = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Type = new ReportInput() { Name = "Type", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNPLDetailBySKU()
        {
            Inputs.Add(inputSku);
            Inputs.Add(Staion);
            Inputs.Add(Type);
        }
        public override void Run()
        {
            string sku = inputSku.Value.ToString();
            string station = Staion.Value.ToString();
            string sqlRun = string.Empty;
            string type = Type.Value.ToString();
            string finalSKU = string.Empty;
            DataTable dt = null;
            OleExec SFCDB = null;
            SFCDB = DBPools["SFCDB"].Borrow();
            string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();
            if (sku.IndexOf(',') != -1)
            {
                List<string> result = sku.Split(new char[] { ',' }).ToList();
                string t = string.Empty;
                for (int i = 0; i < result.Count; i++)
                {
                    t += "'" + result[i].ToString() + "',";
                }
                finalSKU = t.Remove(t.Length - 1, 1);
            }
            else
            {
                finalSKU = "'" + sku + "'";
            }
            try
            {
                if (bu == "VNDCN")
                {
                    if (type == "TOTAL_SN")
                    {
                        sqlRun = $@" SELECT distinct SN.SN,SN.SKUNO,SN.WORKORDERNO,sn.current_station,sn.next_station,pl.pack_no
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO={finalSKU}
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID
                                                 UNION 
                                      SELECT distinct SN.SN,SN.SKUNO,SN.WORKORDERNO,sn.current_station,sn.next_station,pl.pack_no
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN
			                                     WHERE SN.SKUNO not in(SELECT SN.SKUNO
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND SN.SKUNO ={finalSKU}
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID
                                                 GROUP BY SN.SKUNO,WH.WH_NAME)
                                                 AND PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO={finalSKU}
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND SN.SKUNO NOT LIKE '#%'";
                    }
                    else if (type == "TOTAL_PALLET")
                    {
                        sqlRun = $@" SELECT distinct pl.pack_no,SN.WORKORDERNO,COUNT(sp.sn_id) AS QTY,WH.WH_NAME,MAX(sn.edit_time) AS EDIT_TIME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO={finalSKU}
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID
                                                 GROUP BY pl.pack_no,SN.WORKORDERNO,WH.WH_NAME
                                                 UNION 
                                      SELECT distinct pl.pack_no,SN.WORKORDERNO,COUNT(sp.sn_id) AS QTY,'' WH_NAME,MAX(sn.edit_time) AS EDIT_TIME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN
			                                     WHERE SN.SKUNO not in(SELECT SN.SKUNO
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND SN.SKUNO ={finalSKU}
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID
                                                 GROUP BY SN.SKUNO,WH.WH_NAME)
                                                 AND PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO={finalSKU}
                                                 AND SN.NEXT_STATION ='{station}'
                                                 AND SN.SKUNO NOT LIKE '#%'
                                                 GROUP BY pl.pack_no,SN.WORKORDERNO";
                    }
                }
                if (bu == "VNJUNIPER")
                {
                    if (type == "TOTAL_SN")
                    {
                        sqlRun = $@"SELECT distinct SN.SN,SN.SKUNO,SN.WORKORDERNO,sn.current_station,sn.next_station,pl.pack_no
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
			                            AND SP.SN_ID = SN.ID
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID
                                      UNION
                                  SELECT distinct SN.SN,SN.SKUNO,SN.WORKORDERNO,sn.current_station,sn.next_station,pl.pack_no
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE W.GROUPID NOT IN ( SELECT  W.GROUPID 
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO)
                                        AND PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
                                        and o.pono is not null
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO";
                    }
                    else if (type == "TOTAL_PALLET")
                    {
                        sqlRun = $@"  SELECT distinct pl.pack_no,SN.WORKORDERNO,COUNT(sp.sn_id) AS QTY,WH.WH_NAME,MAX(sn.edit_time) AS EDIT_TIME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
			                            AND SP.SN_ID = SN.ID
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID
                                        GROUP BY pl.pack_no,SN.WORKORDERNO,WH.WH_NAME
                                      UNION
                                  SELECT distinct pl.pack_no,SN.WORKORDERNO,COUNT(sp.sn_id) AS QTY,'' WH_NAME,MAX(sn.edit_time) AS EDIT_TIME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE W.GROUPID NOT IN ( SELECT  W.GROUPID 
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO)
                                        AND PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION ='{station}'
                                        AND W.GROUPID={finalSKU}
                                        and o.pono is not null
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        GROUP BY pl.pack_no,SN.WORKORDERNO";
                    }
                }
                ReportTable reportTable = new ReportTable();
                dt = SFCDB.RunSelect(sqlRun).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "SN Status";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
    }
}
