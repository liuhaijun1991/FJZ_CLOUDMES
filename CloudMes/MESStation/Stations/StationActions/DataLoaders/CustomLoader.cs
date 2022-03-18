using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataLoaders
{
    class CustomLoader
    {
        /// <summary>
        /// 從SKU加載Customer
        /// 若Series已裝載則從Series加載
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CustFromSKUDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            
            OleExec sfcdb = Station.SFCDB;
            C_SERIES c_series = null;
            C_CUSTOMER c_customer = null;
            MESStationSession serieSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (serieSession == null)
            {
                MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (skuSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
                }
                SKU sku = (SKU) skuSession.Value;
                if (sku == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
                }
                c_series = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailById(sfcdb, sku.CSeriesId);//sku.CSeriesId
                //是否加入StationSession...
            }
            else
            {
                c_series = (C_SERIES) serieSession.Value;
            }
            if (c_series == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SERIES" }));
            }
            //構建查詢CUSTOMER參數
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("ID", c_series.CUSTOMER_ID);
            List<C_CUSTOMER> customers = new T_C_CUSTOMER(sfcdb, DB_TYPE_ENUM.Oracle).GetCustomerList(parameters, sfcdb);
            if (customers.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "CUSTOMER" }));
            }
            c_customer = customers[0];
            MESStationSession custSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (custSession == null)
            {
                custSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(custSession);
            }
            custSession.Value = c_customer;
            
            Station.AddMessage("MES00000029", new string[] { "CUSTOMER", c_customer.CUSTOMER_NAME }, StationMessageState.Pass);
            
            
        }        

        /// <summary>
        /// 從SKU加載Series
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SeriesFromSKUDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            //加載SeriesSavePoint
            MESStationSession SeriesSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SeriesSession == null)
            {
                SeriesSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(SeriesSession);
            }

            //SKULoadPoint
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
            }


            SKU sku = (SKU)SkuSession.Value;
            if (sku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
            }
            OleExec sfcdb = Station.SFCDB;
            T_C_SERIES t_c_series = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle);
            C_SERIES c_series = t_c_series.GetDetailById(sfcdb, sku.CSeriesId);//sku.CSeriesId
            if (c_series == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SERIES" }));
            }

            SeriesSession.Value = c_series;
            
            Station.AddMessage("MES00000029", new string[] { "SERIES", c_series.SERIES_NAME }, StationMessageState.Pass);
        }

        /// <summary>
        /// 從SKU加載客戶料號
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CUST_PARTNOFromSKUDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            OleExec sfcdb = Station.SFCDB;

            C_SERIES c_series = null;
            C_CUSTOMER c_customer = null;
            MESStationSession serieSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (serieSession == null)
            {
                MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (skuSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
                }
                SKU sku = (SKU)skuSession.Value;
                if (sku == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SKU" }));
                }
                c_series = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailById(sfcdb, sku.CSeriesId);//sku.CSeriesId
                //是否加入StationSession...
            }
            else
            {
                c_series = (C_SERIES)serieSession.Value;
            }
            if (c_series == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SERIES" }));
            }
            //構建查詢CUSTOMER參數
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("ID", c_series.CUSTOMER_ID);
            List<C_CUSTOMER> customers = new T_C_CUSTOMER(sfcdb, DB_TYPE_ENUM.Oracle).GetCustomerList(parameters, sfcdb);
            if (customers.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "CUSTOMER" }));
            }
            c_customer = customers[0];
            MESStationSession custSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (custSession == null)
            {
                custSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(custSession);
            }
            custSession.Value = c_customer;

            Station.AddMessage("MES00000029", new string[] { "CUSTOMER", c_customer.CUSTOMER_NAME }, StationMessageState.Pass);


        }

    }
}
