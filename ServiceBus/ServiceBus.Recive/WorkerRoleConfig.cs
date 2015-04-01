using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Recive
{
    public class WorkerRoleConfig
    {
        private static string logQueueName;
        private static string serviceBusConnectionString;
        private static string logSqlConnectionString;
        private static bool testSpeed;
        private static bool useTrigger;
        private static int logQueueCount;
        private static int threadPerQueueCount;
        private static int receiveBatchCount;
        private static int flushDbBallsCount;
        private static int maxConcurrentCalls;
        private static int testSpeedQueueCount;

        static WorkerRoleConfig()
        {
            UpdateConfig();
        }

        public static void UpdateConfig()
        {
            logQueueName = CloudConfigurationManager.GetSetting("LogQueueName");
            serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            logSqlConnectionString = CloudConfigurationManager.GetSetting("LogSqlConnectionString");
            logQueueCount = Convert.ToInt32(CloudConfigurationManager.GetSetting("LogQueueCount"));
            threadPerQueueCount = Convert.ToInt32(CloudConfigurationManager.GetSetting("ThreadPerQueueCount"));
            receiveBatchCount = Convert.ToInt32(CloudConfigurationManager.GetSetting("ReceiveBatchCount"));
            flushDbBallsCount = Convert.ToInt32(CloudConfigurationManager.GetSetting("FlushDbBallsCount"));
            maxConcurrentCalls = Convert.ToInt32(CloudConfigurationManager.GetSetting("MaxConcurrentCalls"));
            testSpeedQueueCount = Convert.ToInt32(CloudConfigurationManager.GetSetting("TestSpeedQueueCount"));
            testSpeed = Convert.ToBoolean(CloudConfigurationManager.GetSetting("TestSpeed"));
            useTrigger = Convert.ToBoolean(CloudConfigurationManager.GetSetting("UseTrigger"));
        }

        public static string LogQueueName
        {
            get { return logQueueName; }
        }

        public static string ServiceBusConnectionString
        {
            get { return serviceBusConnectionString; }
        }

        public static string LogSqlConnectionString
        {
            get { return logSqlConnectionString; }
        }

        public static int LogQueueCount
        {
            get { return logQueueCount; }
        }

        public static int ThreadPerQueueCount
        {
            get { return threadPerQueueCount; }
        }

        public static int ReceiveBatchCount
        {
            get { return receiveBatchCount; }
        }

        public static int FlushDbBallsCount
        {
            get { return flushDbBallsCount; }
        }

        public static int MaxConcurrentCalls
        {
            get { return maxConcurrentCalls; }
        }

        public static int TestSpeedQueueCount
        {
            get { return testSpeedQueueCount; }
        }

        public static bool TestSpeed
        {
            get { return testSpeed; }
        }

        public static bool Start
        {
            get { return Convert.ToBoolean(CloudConfigurationManager.GetSetting("Start")); }
        }

        public static bool UseTrigger { get { return useTrigger; } }
    }
}
