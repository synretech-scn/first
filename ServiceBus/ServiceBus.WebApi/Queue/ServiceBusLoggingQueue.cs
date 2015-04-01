using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ServiceBus.WebApi.Configuration;
using ServiceBus.WebApi.Infrastructure;
using ServiceBus.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace ServiceBus.WebApi.Queue
{
    public class ServiceBusLoggingQueue : IDisposable
    {
        public const int TOTAL_NUMBER_LOGGING_QUEUES = 1;
        private static readonly Config _config;
        private static readonly List<ServiceBusLoggingQueue> _instanceList;

        private MessagingFactory _messagingFactory;
        private QueueClient _queueClient;
        private int _instanceId;

        static ServiceBusLoggingQueue()
        {
            _config = Singleton<Config>.Instance;
            _instanceList = new List<ServiceBusLoggingQueue>(TOTAL_NUMBER_LOGGING_QUEUES);

            for (int i = 0; i < TOTAL_NUMBER_LOGGING_QUEUES; ++i)
            {
                _instanceList.Add(new ServiceBusLoggingQueue(i));
            }
        }

        public static ServiceBusLoggingQueue GetInstance(int instanceId)
        {
            return _instanceList.ElementAt(instanceId);
        }

        protected ServiceBusLoggingQueue(int instanceId)
        {
            _instanceId = instanceId;
            this.Init();
        }

        private void Init()
        {
            var uri = ServiceBusEnvironment.CreateServiceUri("sb", _config.ServiceNamespace, string.Empty);
            var tokenProvider = SharedSecretTokenProvider.CreateSharedSecretTokenProvider(_config.ServiceIssuer, _config.ServiceSecret);
            var namespaceManager = new NamespaceManager(uri, tokenProvider);

            if (!namespaceManager.QueueExists(_config.LogQueueName + _instanceId))
            {
                namespaceManager.CreateQueue(_config.LogQueueName + _instanceId);
            }

            _messagingFactory = MessagingFactory.Create(uri, TokenProvider.CreateSharedSecretTokenProvider(_config.ServiceIssuer, _config.ServiceSecret));
            _queueClient = _messagingFactory.CreateQueueClient(_config.LogQueueName + _instanceId, ReceiveMode.PeekLock);
        }

        public void PostBalls(Balls balls, String venueId, string ip)
        {
            List<BrokeredMessage> message = new List<BrokeredMessage>();
            foreach (Ball item in balls)
            {
                item.VenueId = venueId;
                if (item.Bowler.Length > 0)
                {
                    //this.Send(item);
                    TraceBall tball = new TraceBall
                    {
                        Bowler = item.Bowler,
                        First = item.First,
                        Foul = item.Foul,
                        Home = item.Home,
                        Id = item.Id,
                        Lane = item.Lane,
                        Pins = item.Pins,
                        Position = item.Position,
                        Score = item.Score,
                        Square = item.Square,
                        Strikes = item.Strikes,
                        VenueId = item.VenueId
                    };
                    //tball.IP = HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString() : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    tball.IP = ip;
                    message.Add(new BrokeredMessage(tball));
                }
            }
            this.Send(message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Send(Ball item)
        {
            if (_queueClient.IsClosed)
            {
                try
                {
                    this.PerformDisposal();
                }
                catch { }

                this.Init();
            }
            _queueClient.Send(new BrokeredMessage(item));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Send(List<BrokeredMessage> message)
        {
            if (_queueClient.IsClosed)
            {
                try
                {
                    this.PerformDisposal();
                }
                catch { }

                this.Init();
            }
            //The maximum size of the batch is the same as the maximum size of a single message (currently 256 Kb).
            _queueClient.SendBatch(message);
        }

        private void PerformDisposal()
        {
            if (_messagingFactory != null)
            {
                _messagingFactory.Close();
            }

            if (_queueClient != null)
            {
                _queueClient.Close();
            }
        }

        public void Dispose()
        {
            this.PerformDisposal();
        }

        public static void DisposeAll()
        {
            foreach (var instance in _instanceList)
            {
                instance.Dispose();
            }
        }
    }
}