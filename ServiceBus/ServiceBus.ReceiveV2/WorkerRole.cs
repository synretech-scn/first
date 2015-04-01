using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using ServiceBus.ReceiveV2.Model;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace ServiceBus.ReceiveV2
{
    public class WorkerRole : RoleEntryPoint
    {
        // 队列的名称
        private string QueueName = CloudConfigurationManager.GetSetting("LogQueueName");
        private string cnString = CloudConfigurationManager.GetSetting("LogSqlConnectionString");
        private string sql = @"INSERT INTO VenueBallLogging([VenueId],[Lane],[Position],[Home],[BallId],[Pins],[First],[Strikes],[Foul],[Square],[Score],[Bowler],[IP]) VALUES(@venueId, @lane, @position, @home, @ballId, @pins, @first, @strikes, @foul, @square, @score, @bowler,@ip)";
        // QueueClient 是线程安全的。建议你进行缓存， 
        // 而不是针对每一个请求重新创建它
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("正在开始处理消息");

            // 启动消息泵，并且将为每个已收到的消息调用回调，在客户端上调用关闭将停止该泵。
            int concurrentCalls = Convert.ToInt32(CloudConfigurationManager.GetSetting("ThreadPerQueue"));
            bool useOptions = Convert.ToBoolean(CloudConfigurationManager.GetSetting("UseMessageOptions"));
            if (!useOptions)
                Client.OnMessage(OnMessageArrived);
            else
            {
                OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = concurrentCalls };
                Client.OnMessage(OnMessageArrived, options);
            }

            CompletedEvent.WaitOne();
        }

        public void OnMessageArrived(BrokeredMessage receivedMessage)
        {
            try
            {
                TraceBall sbBall = GetCloudTraceBall(receivedMessage);
                if (sbBall == null)
                {
                    receivedMessage.Abandon();
                    return;
                }
                using (SqlConnection cn = new SqlConnection(cnString))
                {
                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand(sql, cn);
                    SqlParameter param = new SqlParameter();

                    param.ParameterName = "@venueId";
                    param.Value = sbBall.VenueId;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@lane";
                    param.Value = sbBall.Lane;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@position";
                    param.Value = sbBall.Position;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@home";
                    param.Value = sbBall.Home;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@ballId";
                    param.Value = sbBall.Id;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@pins";
                    param.Value = sbBall.Pins;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@first";
                    param.Value = sbBall.First;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@strikes";
                    param.Value = sbBall.Strikes;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@foul";
                    param.Value = sbBall.Foul;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@square";
                    param.Value = sbBall.Square;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@score";
                    param.Value = sbBall.Score;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@bowler";
                    param.Value = sbBall.Bowler;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@ip";
                    param.Value = sbBall.IP;
                    param.SqlDbType = SqlDbType.NVarChar;
                    sqlCmd.Parameters.Add(param);

                    sqlCmd.ExecuteNonQuery();

                }

                int venueId;
                if (!int.TryParse(sbBall.VenueId, out venueId))
                {
                    receivedMessage.Abandon();
                    return;
                }
                receivedMessage.Complete();
            }
            catch
            {
                // 在此处处理任何处理特定异常的消息
            }
        }
        public override bool OnStart()
        {
            // 设置最大并发连接数 
            ServicePointManager.DefaultConnectionLimit = 12;

            // 如果队列不存在，则创建队列
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // 初始化与 Service Bus 队列的连接
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // 关闭与 Service Bus 队列的连接
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }

        private TraceBall GetCloudTraceBall(BrokeredMessage receivedMessage)
        {
            try
            {
                return receivedMessage.GetBody<TraceBall>();
            }
            catch (SerializationException)
            {
                return null;
            }
        }
    }
}