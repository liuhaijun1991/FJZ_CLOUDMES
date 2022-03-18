using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
//using Newtonsoft.Json;
using System.Threading;

namespace MESInterface.BPD
{
    //public class RecordTestByMQ:taskBase,IDisposable
    //{
    //    private string DB = string.Empty;
    //    string BU = string.Empty;
    //    private OleExec SFCDB = null;
    //    public T_R_TEST_RECORD TestRecord = null;
    //    public T_R_REPAIR_MAIN RepairMain = null;
    //    public T_R_SN RSN = null;
    //    public T_C_ROUTE_DETAIL TCRD = null;
    //    public T_C_ROUTE_DETAIL_DIRECTLINK TCRDDirectLink = null;
    //    public T_R_SN_STATION_DETAIL TRSSD = null;
    //    public ConnectionFactory factory = null;
    //    public IConnection connection = null;
    //    public IModel channel = null;
    //    public string MQUserName = string.Empty;
    //    public string MQPassword = string.Empty;
    //    public string MQHostName = string.Empty;
    //    public int MQPort = 0;
    //    public string ExchangeName = string.Empty;
    //    public string ExchangeType = string.Empty;
    //    public string QueueName = string.Empty;
    //    public string DeadLetterExchangeName = string.Empty;
    //    public string DeadLetterExchangeType = string.Empty;
    //    public string DeadLetterQueueName = string.Empty;
    //    public int MaxRetryTimes = 0;
    //    public int RetryPeriodOfMinutes = 0;
    //    private Timer _timer = null;

    //    public const string OK = "Ok";
    //    public const string ERROR = "Error";
    //    private CountdownEvent cde = new CountdownEvent(1);


    //    public override void init()
    //    {
    //        DB = ConfigGet("DB");
    //        BU = ConfigGet("BU");
    //        MQHostName = ConfigGet("MQHostName");
    //        MQPort = int.Parse(ConfigGet("MQPort"));
    //        MQUserName = ConfigGet("MQUserName");
    //        MQPassword = ConfigGet("MQPassword");
    //        ExchangeName = ConfigGet("ExchangeName");
    //        ExchangeType = ConfigGet("ExchangeType");
    //        QueueName = ConfigGet("QueueName");
    //        DeadLetterExchangeName = ConfigGet("DeadLetterExchangeName");
    //        DeadLetterExchangeType = ConfigGet("DeadLetterExchangeType");
    //        DeadLetterQueueName = ConfigGet("DeadLetterQueueName");
    //        MaxRetryTimes = int.Parse(ConfigGet("MaxRetryTimes"));
    //        RetryPeriodOfMinutes = int.Parse(ConfigGet("RetryPeriodOfMinutes"));


    //        SFCDB = new OleExec(DB, true);
    //        Output.UI = new RecordTestByMQ_UI(this);
    //        TestRecord = new T_R_TEST_RECORD(SFCDB, DB_TYPE_ENUM.Oracle);
    //        RepairMain = new T_R_REPAIR_MAIN(SFCDB, DB_TYPE_ENUM.Oracle);
    //        TCRD = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
    //        TCRDDirectLink = new T_C_ROUTE_DETAIL_DIRECTLINK(SFCDB, DB_TYPE_ENUM.Oracle);
    //        RSN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
    //        TRSSD = new T_R_SN_STATION_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
    //        factory = new ConnectionFactory()
    //        {
    //            UserName = MQUserName,
    //            Password = MQPassword,
    //            HostName = MQHostName,
    //            Port = MQPort
    //        };
    //        connection = factory.CreateConnection();
    //        channel = connection.CreateModel();
    //        base.init();
    //    }

    //    public string StationMapping(string OriginalStation)
    //    {
    //        if (OriginalStation.StartsWith("ROBAT_RXI"))
    //        {
    //            return "XRAY_RXI";
    //        }
    //        if (OriginalStation.StartsWith("ROBAT_S1"))
    //        {
    //            return "ICT_S1";
    //        }
    //        if (OriginalStation.Contains("3DX"))
    //        {
    //            return "XRAY_3DX";
    //        }
    //        else
    //        {
    //            return OriginalStation;
    //        }
    //    }

    //    public string LineMapping(string DeviceName)
    //    {
    //        switch (DeviceName.ToUpper())
    //        {
    //            case "3DX_01":
    //                return "A13XRAY3DX01";
    //            case "3DX_02":
    //                return "A13XRAY3DX02";
    //            case "RXI_01":
    //                return "A13XRAYRXI01";
    //            case "RXI_02":
    //                return "A13XRAYRXI02";
    //            case "FXSICT1":
    //                return "A13ICT";
    //            case "S1_01":
    //                return "A13ICTS101";
    //            case "S1_02":
    //                return "A13ICTS102";
    //            default:
    //                return DeviceName;
    //        }
    //    }

    //    public override void Start()
    //    {
    //        DoSomething(SFCDB, BU, "BPD_RecordTestByMQ", "BPD_RecordTestByMQ", "Record Test By MQ Fail", DoRecordTest);
    //    }


    //    private void ConsumeDeadLetterQueue(object state)
    //    {
    //        IModel channel = ((Tuple<IModel, string>)state).Item1;
    //        string QueueName = ((Tuple<IModel, string>)state).Item2;
    //        BasicGetResult result = null;

    //        try
    //        {
    //            //重置等待計數器，在調用 Signal 方法之前，正常隊列的消息都無法被消費，因為使用了 Wait 方法，目的是防止消息被快速的在死信隊列和正常隊列之間消費
    //            cde.Reset();
    //            //遍歷死信隊列所有的消息
    //            while ((result = channel.BasicGet(QueueName, false)) != null)
    //            {
    //                //只處理來自於死信隊列方式的消息（如果是死信隊列的消息，那麼消息的屬性頭部中一定有 x-death 這個鍵）
    //                if (result.BasicProperties.Headers.ContainsKey("x-death"))
    //                {
    //                    //獲取是從哪個隊列發到死信隊列的，這是用來將消息發送回去
    //                    var originalExchange = GetString((byte[])result.BasicProperties.Headers["x-first-death-exchange"]);
    //                    //判斷死信隊列中的消息的屬性頭中是否有 retryTimes 這個屬性，如果沒有的話就添加一下並初始化為0
    //                    if (!result.BasicProperties.Headers.ContainsKey("retryTimes"))
    //                    {
    //                        result.BasicProperties.Headers.Add("retryTimes", 0);
    //                    }
    //                    //獲取消息的已重試次數
    //                    var retryTimes = Int32.Parse(result.BasicProperties.Headers["retryTimes"].ToString());
    //                    //判斷消息重試次數與設定的最大重試次數，如果沒有達到最大重試次數，那麼就將消息發回給正常隊列並確認消息已被消費；否則直接確認消息已被消費
    //                    if (retryTimes < MaxRetryTimes)
    //                    {
    //                        result.BasicProperties.Headers["retryTimes"] = retryTimes + 1;
    //                        channel.BasicPublish(originalExchange.ToString(), "", result.BasicProperties, result.Body);
    //                        channel.BasicAck(result.DeliveryTag, false);
    //                    }
    //                    else
    //                    {
    //                        channel.BasicAck(result.DeliveryTag, false);
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        { }
    //        finally
    //        {
    //            if (cde.CurrentCount > 0)
    //            {
    //                cde.Signal(cde.CurrentCount);
    //            }
    //        }

    //    }

    //    private void DoRecordTest()
    //    {
    //        //多次啟動會銷毀之前啟動的定時任務
    //        if (_timer != null)
    //        {
    //            _timer.Dispose();
    //        }
    //        //首先發出釋放信號，避免timer 沒有執行導致下面正常接收消息的時候每次都要等到15秒的超時時間後才消費
    //        if (cde.CurrentCount > 0)
    //        {
    //            cde.Signal(cde.CurrentCount);
    //        }

    //        //每隔指定時間掃描一次死信隊列，如果還沒達到重試上限，則將消息插入到正常隊列中繼續消費
    //        _timer = new Timer(ConsumeDeadLetterQueue, new Tuple<IModel, string>(channel, DeadLetterQueueName), TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(RetryPeriodOfMinutes));

    //        //聲明死信隊列
    //        channel.ExchangeDeclare(DeadLetterExchangeName, DeadLetterExchangeType, true, false);
    //        channel.QueueDeclare(DeadLetterQueueName, true, false, false);
    //        channel.QueueBind(DeadLetterQueueName, DeadLetterExchangeName, "");
    //        //聲明正常隊列並且將死信隊列添加到正常隊列上
    //        channel.ExchangeDeclare(ExchangeName, ExchangeType, true, false);
    //        channel.QueueDeclare(QueueName, true, false, false, new Dictionary<string, object> {
    //            { "x-dead-letter-exchange",DeadLetterExchangeName}
    //        });
    //        channel.QueueBind(QueueName, ExchangeName, "");
    //        channel.BasicQos(0, 1, false);

    //        connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

    //        var consumer = new EventingBasicConsumer(channel);
    //        //多次啟動會先解除事件處理，再添加上事件處理
    //        consumer.Received -= Consumer_Received;
    //        consumer.Received += Consumer_Received;
    //        channel.BasicConsume(QueueName, false, consumer);
    //    }

    //    private void Consumer_Received(object sender, BasicDeliverEventArgs ea)
    //    {
    //        //設定等待信號通知再繼續往下走，超時時間為 15 秒
    //        if (cde.Wait(TimeSpan.FromSeconds(15)))
    //        {
    //            try
    //            {
    //                var messageId = ea.BasicProperties.MessageId ?? "";
    //                var result = HandleMessage(GetString(ea.Body), messageId);
    //                if (result.Equals(OK))
    //                {
    //                    channel.BasicAck(ea.DeliveryTag, false);
    //                }
    //                else
    //                {
    //                    channel.BasicReject(ea.DeliveryTag, false);
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                channel.BasicReject(ea.DeliveryTag, false);
    //            }
    //        }
    //    }

    //    private string GetString(byte[] content)
    //    {
    //        return Encoding.UTF8.GetString(content);
    //    }

    //    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    //    {
    //    }

    //    public string HandleMessage(string MessageContent, string MessageId)
    //    {
    //        if (MessageContent.Length == 0)
    //        {
    //            return OK;
    //        }

    //        TestMessage message = null;
    //        try
    //        {
    //            message = JsonConvert.DeserializeObject<TestMessage>(MessageContent);
    //        }
    //        catch (Exception)
    //        {
    //            return OK;
    //        }

    //        if (message == null)
    //        {
    //            return OK;
    //        }

    //        Dictionary<string, List<C_ROUTE_DETAIL>> routeCache = new Dictionary<string, List<C_ROUTE_DETAIL>>();
    //        R_TEST_RECORD record = null;


    //        R_SN SN = RSN.GetSN(message.Sn, SFCDB);
    //        if (SN != null)
    //        {
    //            var station = StationMapping(message.Station);
    //            //如果R_SN中的NextStation不等於當前消息中的Station   
    //            if (!SN.NEXT_STATION.Equals(station))
    //            {
    //                //嘗試從緩存中加載
    //                List<C_ROUTE_DETAIL> routeDetails = null;
    //                if (routeCache.ContainsKey(SN.ROUTE_ID))
    //                {
    //                    routeDetails = routeCache[SN.ROUTE_ID];
    //                }
    //                else
    //                {
    //                    routeDetails = TCRD.GetByRouteIdOrderBySEQASC(SN.ROUTE_ID, SFCDB);
    //                    routeCache.Add(SN.ROUTE_ID, routeDetails);
    //                }
    //                #region 20191122 Route中有跳站時根據原本的設計緊緊看NextStation,這是不嚴謹的
    //                //獲取當前工站名稱
    //                string VarCurrentStation = SN.CURRENT_STATION;
    //                //獲取當前工站在Route中對應的ID
    //                C_ROUTE_DETAIL CSID = routeDetails.Find(t => t.STATION_NAME == VarCurrentStation);
    //                if (CSID != null)//肯定不空,所以不用else
    //                {
    //                    string VarCurrentStationRouteID = CSID.ID;
    //                    //getDetailDirectLink先置空
    //                    List<C_ROUTE_DETAIL_DIRECTLINK> getDetailDirectLink = null;
    //                    //根據當前工站ID查C_ROUTE_DETAIL_DIRECTLINK
    //                    getDetailDirectLink = TCRDDirectLink.GetByDetailId(VarCurrentStationRouteID, SFCDB);
    //                    if (getDetailDirectLink.Count == 0)//說明沒有跳站
    //                    {
    //                        //如果消息裡面的站位在流程裡面沒有找到，那麼丟棄消息
    //                        C_ROUTE_DETAIL rd = routeDetails.Find(t => t.STATION_NAME == station);
    //                        if (rd == null)
    //                        {
    //                            return OK;
    //                        }

    //                        //如果產品的 NEXT_STATION 有問題，繼續並且丟棄消息
    //                        C_ROUTE_DETAIL nextStation = routeDetails.Find(t => t.STATION_NAME == SN.NEXT_STATION);
    //                        if (nextStation == null)
    //                        {
    //                            return OK;
    //                        }

    //                        //如果產品的 NEXT_STATION 已經在消息裡面站位之後，表示產品已經過站了，則丟棄消息
    //                        if (rd.SEQ_NO < nextStation.SEQ_NO && TestRecord.GetLastTestRecord(SN.SN, station, SFCDB) != null)
    //                        {
    //                            return OK;
    //                        }

    //                        //如果消息裡面的站位在產品的 NEXT_STATION 之後，則保留消息之後再消費
    //                        if (rd.SEQ_NO > nextStation.SEQ_NO)
    //                        {
    //                            return ERROR;
    //                        }
    //                    }
    //                    else//說明有跳站
    //                    {
    //                        //用當前工站ID查DirectLink中的DirectLinkID
    //                        C_ROUTE_DETAIL_DIRECTLINK DirectlinkID = getDetailDirectLink.Find(t => t.C_ROUTE_DETAIL_ID == VarCurrentStationRouteID);
    //                        if (DirectlinkID != null)//肯定不空,所以不用else
    //                        {
    //                            //獲取當前工站可以跳站的工站名稱在Route中對應的ID
    //                            C_ROUTE_DETAIL CStation = routeDetails.Find(t => t.ID == DirectlinkID.DIRECTLINK_ROUTE_DETAIL_ID);
    //                            if (CStation != null)
    //                            {
    //                                string RouteStationName = CStation.STATION_NAME;
    //                                if (RouteStationName.Equals(station))
    //                                {
    //                                    //目的是判斷到是跳站,更新r_sn的CurrentStation and NextStation,以便不變更updatestatus的邏輯
    //                                    int result = RSN.TiaoZhanUpdateCurrentNextStation(SN.ID, SN.CURRENT_STATION, RouteStationName, SFCDB);
    //                                    if (result <= 0)
    //                                    {
    //                                        WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestMessage", "RecordTestMessage", "RecordTestMessage", MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SN.SN, "UPDATE" }), "update r_sn set current_station=:CurrentStation,next_station=:NextStation where id=:snid", "Interface");
    //                                        return ERROR;
    //                                    }
    //                                }
    //                                else//幾乎不可能會null
    //                                {
    //                                    //如果消息裡面的站位在流程裡面沒有找到，那麼丟棄消息
    //                                    C_ROUTE_DETAIL rd = routeDetails.Find(t => t.STATION_NAME == station);
    //                                    if (rd == null)
    //                                    {
    //                                        return OK;
    //                                    }

    //                                    //如果產品的 NEXT_STATION 有問題，繼續並且丟棄消息
    //                                    C_ROUTE_DETAIL nextStation = routeDetails.Find(t => t.STATION_NAME == SN.NEXT_STATION);
    //                                    if (nextStation == null)
    //                                    {
    //                                        //丟棄消息
    //                                        return OK;
    //                                    }

    //                                    //如果產品的 NEXT_STATION 已經在消息裡面站位之後，表示產品已經過站了，則丟棄消息
    //                                    if (rd.SEQ_NO < nextStation.SEQ_NO && TestRecord.GetLastTestRecord(SN.SN, station, SFCDB) != null)
    //                                    {
    //                                        //丟棄消息
    //                                        return OK;
    //                                    }

    //                                    //如果產品的跳站站位在消息裡面的站位之前，則消息留著之後再消費
    //                                    if (rd.SEQ_NO > CStation.SEQ_NO)
    //                                    {
    //                                        return ERROR;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                #endregion
    //            }


    //            record = new R_TEST_RECORD();
    //            record.ID = TestRecord.GetNewID(BU, SFCDB);
    //            record.R_SN_ID = SN.ID;
    //            record.SN = SN.SN;
    //            record.STATE = message.Result;
    //            record.TEGROUP = station;
    //            record.TESTATION = station;
    //            record.MESSTATION = station;
    //            record.DEVICE = LineMapping(message.Machine);
    //            record.STARTTIME = message.TestStart;
    //            record.ENDTIME = message.TestEnd;
    //            record.DETAILTABLE = $@"from message queue,message id:{MessageId}";
    //            record.EDIT_EMP = message.Operator;
    //            record.EDIT_TIME = TestRecord.GetDBDateTime(SFCDB);
    //            record.TESTINFO = message.Information;

    //            //WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestFile", "RecordTestFile", "RecordTestFile", $@"Insert into R_TEST_RECORD", "", "A0225204");

    //            if (record.STATE.Equals("PASS"))
    //            {   //過站
    //                RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "PASS", record.EDIT_EMP, SFCDB);
    //                //success++;
    //            }
    //            else
    //            {
    //                RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "FAIL", record.EDIT_EMP, SFCDB);
    //                //RSN.RecordPassStationDetail(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, SFCDB, "1");
    //                //RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, record.STATE, record.EDIT_EMP, SFCDB);
    //            }
    //            try
    //            {
    //                TestRecord.InsertTestRecord(record, SFCDB);
    //            }
    //            catch (Exception e)
    //            {
    //                WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestMessage", "RecordTestMessage", "RecordTestMessage", $@"{e.Message}", "insert into r_test_record", "Interface");
    //            }

    //            //寫UPH，良率
    //            RSN.RecordUPH(SN.WORKORDERNO, 1, SN.SN, record.STATE, record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);
    //            RSN.RecordYieldRate(SN.WORKORDERNO, 1, SN.SN, record.STATE, record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);

    //            //else
    //            //{
    //            //    //寫不良
    //            //    SN.REPAIR_FAILED_FLAG = "1";
    //            //    RSN.Update(SN, SFCDB);
    //            //    RepairMain.Insert(SN, Station, record.DEVICE, record.EDIT_EMP, record.ENDTIME, BU, SFCDB);
    //            //}
    //        }

    //        return OK;
    //    }

    //    public void Dispose()
    //    {
    //        _timer.Dispose();
    //        GC.Collect();
    //    }
    //}

    //class TestMessage
    //{
    //    [JsonProperty(PropertyName ="P/N")]
    //    public string PartNo {get;set;}
    //    [JsonProperty(PropertyName ="S/N")]
    //    public string Sn { get; set; }
    //    public string Result { get; set; }
    //    public string Station { get; set; }
    //    public string Machine { get; set; }
    //    [JsonProperty(PropertyName ="OP")]
    //    public string Operator { get; set; }
    //    public string Shift { get; set; }
    //    public DateTime? TestStart { get; set; }
    //    public DateTime? TestEnd { get; set; }
    //    public long TestTime { get; set; }
    //    public string Information { get; set; }
    //    public string Fault { get; set; }
    //}
}
