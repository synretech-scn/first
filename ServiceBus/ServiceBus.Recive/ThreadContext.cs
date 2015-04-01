using Microsoft.ServiceBus.Messaging;
using ServiceBus.Recive.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Recive
{
    public class ThreadContext
    {
        private readonly int instanceId;
        private readonly int queueId;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly List<TraceBall> traceBalls = new List<TraceBall>();
        private readonly Stopwatch stopwatch = new Stopwatch();

        private QueueClient logClientBalls;

        public ThreadContext(int instanceId, int queueId)
        {
            this.instanceId = instanceId;
            this.queueId = queueId;
        }

        public void Run()
        {
            logClientBalls = QueueClient.CreateFromConnectionString(WorkerRoleConfig.ServiceBusConnectionString, WorkerRoleConfig.LogQueueName + queueId);

            try
            {
                if (WorkerRoleConfig.TestSpeed)
                {
                    if (WorkerRoleConfig.UseTrigger)
                    {
                        this.RunTriggerCountAsync(this.cancellationTokenSource.Token).Wait();

                    }
                    else
                    {
                        if (WorkerRoleConfig.ReceiveBatchCount <= 1)
                            this.RunCountAsync(this.cancellationTokenSource.Token).Wait();
                        else
                            this.RunBatchCountAsync(this.cancellationTokenSource.Token).Wait();
                    }
                }
                else
                {
                    if (WorkerRoleConfig.ReceiveBatchCount <= 1)
                        this.RunReceiveAsync(this.cancellationTokenSource.Token).Wait();
                    else
                        this.RunReceiveBatchAsync(this.cancellationTokenSource.Token).Wait();
                }
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
        }

        private async Task RunTriggerCountAsync(CancellationToken cancellationToken)
        {
            int count = WorkerRoleConfig.TestSpeedQueueCount;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            OnMessageOptions options = new OnMessageOptions { MaxConcurrentCalls = WorkerRoleConfig.MaxConcurrentCalls, AutoComplete = false };
            logClientBalls.OnMessage((receiveMessage) =>
            {
                if (receiveMessage == null)
                    return;
                else
                {
                    receiveMessage.Complete();
                    --count;
                    if (count <= 0)
                    {
                        LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount, sw.Elapsed.TotalSeconds, "trigger");
                        count = WorkerRoleConfig.TestSpeedQueueCount;
                        sw.Restart();
                    }
                }
            }, options);
            cancellationToken.WaitHandle.WaitOne();
            LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount - count, sw.Elapsed.TotalSeconds, "trigger");
            await Task.Delay(TimeSpan.FromTicks(1));
        }
        private async Task RunBatchCountAsync(CancellationToken cancellationToken)
        {
            int count = WorkerRoleConfig.TestSpeedQueueCount;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    List<BrokeredMessage> receivedBalls = logClientBalls.ReceiveBatch(WorkerRoleConfig.ReceiveBatchCount).ToList();

                    if (receivedBalls == null)
                        continue;
                    else
                    {
                        foreach (var receivedBall in receivedBalls)
                        {
                            receivedBall.Complete();
                            --count;
                            if (count <= 0)
                            {
                                LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount, sw.Elapsed.TotalSeconds, "poll-batch");
                                count = WorkerRoleConfig.TestSpeedQueueCount;
                                sw.Restart();
                            }
                        }
                    }
                }
                catch (MessageLockLostException eLost)
                {
                    //Trace("Error MessageLockLostException (Player): " + eLost.Message, LoggingLevel.Warning);
                }
                catch (MessagingException e)
                {
                    //if (!e.IsTransient)
                    //Trace("Error MessagingException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (OperationCanceledException e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error OperationCanceledException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (Exception e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error Exception (Player): " + e.ToString(), LoggingLevel.Error);
                }
            }
            LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount - count, sw.Elapsed.TotalSeconds, "poll-batch");
            await Task.Delay(TimeSpan.FromTicks(1));
        }

        private async Task RunCountAsync(CancellationToken cancellationToken)
        {
            int count = WorkerRoleConfig.TestSpeedQueueCount;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BrokeredMessage receivedBall = logClientBalls.Receive();

                    if (receivedBall == null)
                        continue;
                    else
                    {
                        receivedBall.Complete();
                        --count;
                        if (count <= 0)
                        {
                            LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount, sw.Elapsed.TotalSeconds, "poll");
                            count = WorkerRoleConfig.TestSpeedQueueCount;
                            sw.Restart();
                        }
                    }
                }
                catch (MessageLockLostException eLost)
                {
                    //Trace("Error MessageLockLostException (Player): " + eLost.Message, LoggingLevel.Warning);
                }
                catch (MessagingException e)
                {
                    //if (!e.IsTransient)
                    //Trace("Error MessagingException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (OperationCanceledException e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error OperationCanceledException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (Exception e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error Exception (Player): " + e.ToString(), LoggingLevel.Error);
                }
            }
            LogCountToDb(WorkerRoleConfig.TestSpeedQueueCount - count, sw.Elapsed.TotalSeconds, "poll");
            await Task.Delay(TimeSpan.FromTicks(1));
        }

        private async Task RunReceiveAsync(CancellationToken cancellationToken)
        {
            stopwatch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BrokeredMessage receivedBall = logClientBalls.Receive();

                    if (receivedBall == null)
                        continue;

                    TraceBall sbBall = GetCloudTraceBall(receivedBall);
                    if (sbBall == null)
                    {
                        sbBall = new TraceBall
                        {
                            Bowler = "NULL",
                            First = "NULL",
                            Foul = "NULL",
                            Home = "NULL",
                            Id = "NULL",
                            IP = "NULL",
                            Lane = "NULL",
                            Pins = "NULL",
                            Position = "NULL",
                            Score = "NULL",
                            Square = "NULL",
                            Strikes = "NULL",
                            VenueId = "NULL",
                        };
                    }
                    traceBalls.Add(sbBall);
                    TimeToLog();
                    receivedBall.Complete();

                }
                catch (MessageLockLostException eLost)
                {
                    //Trace("Error MessageLockLostException (Player): " + eLost.Message, LoggingLevel.Warning);
                }
                catch (MessagingException e)
                {
                    //if (!e.IsTransient)
                    //Trace("Error MessagingException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (OperationCanceledException e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error OperationCanceledException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (Exception e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error Exception (Player): " + e.ToString(), LoggingLevel.Error);
                }
            }
            stopwatch.Stop();
            LogTraceBallsToDb();
            await Task.Delay(TimeSpan.FromTicks(1));
        }

        private async Task RunReceiveBatchAsync(CancellationToken cancellationToken)
        {
            stopwatch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    List<BrokeredMessage> receivedBalls = logClientBalls.ReceiveBatch(WorkerRoleConfig.ReceiveBatchCount).ToList();
                    foreach (var receivedBall in receivedBalls)
                    {
                        TraceBall sbBall = GetCloudTraceBall(receivedBall);
                        if (sbBall == null)
                        {
                            sbBall = new TraceBall
                            {
                                Bowler = "NULL",
                                First = "NULL",
                                Foul = "NULL",
                                Home = "NULL",
                                Id = "NULL",
                                IP = "NULL",
                                Lane = "NULL",
                                Pins = "NULL",
                                Position = "NULL",
                                Score = "NULL",
                                Square = "NULL",
                                Strikes = "NULL",
                                VenueId = "NULL",
                            };
                        }
                        traceBalls.Add(sbBall);
                        TimeToLog();
                        receivedBall.Complete();
                    }
                }
                catch (MessageLockLostException eLost)
                {
                    //Trace("Error MessageLockLostException (Player): " + eLost.Message, LoggingLevel.Warning);
                }
                catch (MessagingException e)
                {
                    //if (!e.IsTransient)
                    //Trace("Error MessagingException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (OperationCanceledException e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error OperationCanceledException (Player): " + e.Message, LoggingLevel.Error);

                }
                catch (Exception e)
                {
                    //if (!cancellationToken.IsCancellationRequested)
                    //Trace("Error Exception (Player): " + e.ToString(), LoggingLevel.Error);
                }
            }
            stopwatch.Stop();
            LogTraceBallsToDb();
            await Task.Delay(TimeSpan.FromTicks(1));
        }

        private Ball GetCloudBall(BrokeredMessage receivedMessage)
        {
            try
            {
                return receivedMessage.GetBody<Ball>();
            }
            catch (SerializationException ex)
            {
                //receivedMessage.DeadLetter("GetCloudBall", ex.ToString());
                return null;
            }
        }

        private TraceBall GetCloudTraceBall(BrokeredMessage receivedMessage)
        {
            try
            {
                return receivedMessage.GetBody<TraceBall>();
            }
            catch (SerializationException ex)
            {
                //receivedMessage.DeadLetter("GetCloudBall", ex.ToString());
                return null;
            }
        }

        private void LogCountToDb(int count, double seconds, string type)
        {
            string sql = @"INSERT INTO LoggingCount([Count],[Seconds],[Type]) VALUES(@count, @seconds, @type)";
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = WorkerRoleConfig.LogSqlConnectionString;
                cn.Open();

                SqlCommand sqlCmd = new SqlCommand(sql, cn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@count";
                param.Value = count;
                param.SqlDbType = SqlDbType.Int;
                sqlCmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@seconds";
                param.Value = seconds;
                param.SqlDbType = SqlDbType.Float;
                sqlCmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@type";
                param.Value = type;
                param.SqlDbType = SqlDbType.NVarChar;
                sqlCmd.Parameters.Add(param);

                sqlCmd.ExecuteNonQuery();
            }
        }

        private void TimeToLog()
        {
            if (traceBalls.Count >= WorkerRoleConfig.FlushDbBallsCount)
            {
                LogTraceBallsToDb();
                stopwatch.Restart();
            }
            else
            {
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    LogTraceBallsToDb();
                    stopwatch.Restart();
                }
            }
        }

        private void LogTraceBallsToDb()
        {
            if (traceBalls.Count == 0)
                return;

            DataTable table = new DataTable("VenueBallLogging");

            DataColumn loggingId = new DataColumn();
            loggingId.ColumnName = "Id";
            loggingId.DataType = System.Type.GetType("System.Int32");
            loggingId.AutoIncrement = true;
            table.Columns.Add(loggingId);

            DataColumn venueId = new DataColumn();
            venueId.ColumnName = "VenueId";
            venueId.DataType = System.Type.GetType("System.String");
            table.Columns.Add(venueId);

            DataColumn lane = new DataColumn();
            lane.ColumnName = "Lane";
            lane.DataType = System.Type.GetType("System.String");
            table.Columns.Add(lane);

            DataColumn position = new DataColumn();
            position.ColumnName = "Position";
            position.DataType = System.Type.GetType("System.String");
            table.Columns.Add(position);

            DataColumn home = new DataColumn();
            home.ColumnName = "Home";
            home.DataType = System.Type.GetType("System.String");
            table.Columns.Add(home);

            DataColumn ballId = new DataColumn();
            ballId.ColumnName = "BallId";
            ballId.DataType = System.Type.GetType("System.String");
            table.Columns.Add(ballId);

            DataColumn pins = new DataColumn();
            pins.ColumnName = "Pins";
            pins.DataType = System.Type.GetType("System.String");
            table.Columns.Add(pins);

            DataColumn first = new DataColumn();
            first.ColumnName = "First";
            first.DataType = System.Type.GetType("System.String");
            table.Columns.Add(first);

            DataColumn strikes = new DataColumn();
            strikes.ColumnName = "Strikes";
            strikes.DataType = System.Type.GetType("System.String");
            table.Columns.Add(strikes);

            DataColumn foul = new DataColumn();
            foul.ColumnName = "Foul";
            foul.DataType = System.Type.GetType("System.String");
            table.Columns.Add(foul);

            DataColumn square = new DataColumn();
            square.ColumnName = "Square";
            square.DataType = System.Type.GetType("System.String");
            table.Columns.Add(square);

            DataColumn score = new DataColumn();
            score.ColumnName = "Score";
            score.DataType = System.Type.GetType("System.String");
            table.Columns.Add(score);

            DataColumn bowler = new DataColumn();
            bowler.ColumnName = "Bowler";
            bowler.DataType = System.Type.GetType("System.String");
            table.Columns.Add(bowler);

            DataColumn createdDateTime = new DataColumn();
            createdDateTime.ColumnName = "CreatedDateTime";
            createdDateTime.DataType = System.Type.GetType("System.DateTime");
            table.Columns.Add(createdDateTime);

            DataColumn ip = new DataColumn();
            ip.ColumnName = "IP";
            ip.DataType = System.Type.GetType("System.String");
            table.Columns.Add(ip);

            DataRow row;
            foreach (var traceBall in traceBalls)
            {
                row = table.NewRow();
                row["VenueId"] = traceBall.VenueId;
                row["Lane"] = traceBall.Lane;
                row["Position"] = traceBall.Position;
                row["Home"] = traceBall.Home;
                row["BallId"] = traceBall.Id;
                row["Pins"] = traceBall.Pins;
                row["First"] = traceBall.First;
                row["Strikes"] = traceBall.Strikes;
                row["Foul"] = traceBall.Foul;
                row["Square"] = traceBall.Square;
                row["Score"] = traceBall.Score;
                row["Bowler"] = traceBall.Bowler;
                row["IP"] = traceBall.IP;
                table.Rows.Add(row);
            }
            table.AcceptChanges();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(WorkerRoleConfig.LogSqlConnectionString))
            {
                bulkCopy.DestinationTableName = "dbo.VenueBallLogging";

                try
                {
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
