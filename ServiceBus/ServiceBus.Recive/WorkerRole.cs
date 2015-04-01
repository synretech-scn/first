using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace ServiceBus.Recive
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly Dictionary<Thread, ThreadContext> threadWorkers = new Dictionary<Thread, ThreadContext>();
        private WorkerRoleStatus status = WorkerRoleStatus.Stopped;

        public override void Run()
        {
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            //设置最大并发连接数
            ServicePointManager.DefaultConnectionLimit = 12;

            // 有关处理配置更改的信息，
            // 请参见 http://go.microsoft.com/fwlink/?LinkId=166357 上的 MSDN 主题。

            bool result = base.OnStart();

            return result;
        }

        public override void OnStop()
        {
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            EmptyThreadWorkers();

            base.OnStop();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (WorkerRoleConfig.Start)
                    {
                        if (status == WorkerRoleStatus.Stopped)
                        {
                            if (threadWorkers.Count == 0)
                            {
                                if (WorkerRoleConfig.TestSpeed)
                                {
                                    //Single thread  to test queue receive speed.
                                    CreateThreads(1, 1);
                                }
                                else
                                {
                                    CreateThreads();
                                }
                            }
                            status = WorkerRoleStatus.Running;
                        }
                        StartThreads();
                        await Task.Delay(1000);
                    }
                    else
                    {
                        if (status == WorkerRoleStatus.Running)
                        {
                            EmptyThreadWorkers();
                            status = WorkerRoleStatus.Stopped;
                        }
                        WorkerRoleConfig.UpdateConfig();
                        await Task.Delay(1000);
                    }
                }
                catch (Exception e) { }
            }

            status = WorkerRoleStatus.Stopped;
        }

        private void StartThreads()
        {
            foreach (var worker in threadWorkers)
            {
                if (!worker.Key.IsAlive)
                    worker.Key.Start();
            }
        }

        private void StopThreads()
        {
            foreach (var worker in threadWorkers)
            {
                if (worker.Key.IsAlive)
                    worker.Value.Stop();
            }
        }

        private void CreateThread(int instanceId, int queueId)
        {
            ThreadContext context = new ThreadContext(instanceId, queueId);
            Thread thread = new Thread(context.Run);
            threadWorkers.Add(thread, context);
        }

        private void CreateThreads(int logQueueCount = 0, int threadPerQueueCount = 0)
        {
            if (logQueueCount == 0 || threadPerQueueCount == 0)
            {
                logQueueCount = WorkerRoleConfig.LogQueueCount;
                threadPerQueueCount = WorkerRoleConfig.ThreadPerQueueCount;
            }
            for (int i = 0; i < logQueueCount; ++i)
            {
                for (int j = 0; j < threadPerQueueCount; ++j)
                {
                    CreateThread(j, i);
                }
            }
        }

        private void EmptyThreadWorkers()
        {
            StopThreads();
            threadWorkers.Clear();
        }
    }

    public enum WorkerRoleStatus
    {
        Starting,
        Started,
        Running,
        Stopping,
        Stopped
    }
}
