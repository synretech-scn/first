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
    public class ServiceBusQueue : IDisposable
    {
        public const int TOTAL_NUMBER_QUEUES = 41;
        private static readonly Config _config;
        private static readonly List<ServiceBusQueue> _instanceList;

        private MessagingFactory _messagingFactory;
        private QueueClient _queueClient;
        private int _instanceId;

        static ServiceBusQueue()
        {
            _config = Singleton<Config>.Instance;
            _instanceList = new List<ServiceBusQueue>(TOTAL_NUMBER_QUEUES);

            for (int i = 0; i < TOTAL_NUMBER_QUEUES; ++i)
            {
                _instanceList.Add(new ServiceBusQueue(i));
            }
        }

        public static ServiceBusQueue GetInstance(int instanceId)
        {
            return _instanceList.ElementAt(instanceId);
        }

        protected ServiceBusQueue(int instanceId)
        {
            _instanceId = instanceId;
            this.Init();
        }

        private void Init()
        {
            var uri = ServiceBusEnvironment.CreateServiceUri("sb", _config.ServiceNamespace, string.Empty);
            var tokenProvider = SharedSecretTokenProvider.CreateSharedSecretTokenProvider(_config.ServiceIssuer, _config.ServiceSecret);
            var namespaceManager = new NamespaceManager(uri, tokenProvider);

            if (!namespaceManager.QueueExists(_config.QueueName + _instanceId))
            {
                namespaceManager.CreateQueue(_config.QueueName + _instanceId);
            }

            _messagingFactory = MessagingFactory.Create(uri, TokenProvider.CreateSharedSecretTokenProvider(_config.ServiceIssuer, _config.ServiceSecret));
            _queueClient = _messagingFactory.CreateQueueClient(_config.QueueName + _instanceId, ReceiveMode.PeekLock);
        }

        public void PostBalls(Balls balls, String venueId)
        {
            foreach (Ball item in balls)
            {
                item.VenueId = venueId;
                if (item.Bowler.Length > 0)
                    this.Send(item);
            } // end foreach
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
            //_queueClient.Send(new BrokeredMessage(item));
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

            _instanceList[_instanceId] = null;
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