using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceBus.WebApi.Configuration
{
    /// <summary>
    /// Represents a NopConfig
    /// </summary>
    public partial class Config : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new Config();
            var serviceBus = section.SelectSingleNode("ServiceBus");

            if (serviceBus != null && serviceBus.Attributes != null)
            {
                var attribute = serviceBus.Attributes["ServiceNamespace"];
                if (attribute != null)
                    config.ServiceNamespace = attribute.Value;

                attribute = serviceBus.Attributes["ServiceIssuer"];
                if (attribute != null)
                    config.ServiceIssuer = attribute.Value;

                attribute = serviceBus.Attributes["ServiceSecret"];
                if (attribute != null)
                    config.ServiceSecret = attribute.Value;

                attribute = serviceBus.Attributes["QueueName"];
                if (attribute != null)
                    config.QueueName = attribute.Value;

                attribute = serviceBus.Attributes["LogQueueName"];
                if (attribute != null)
                    config.LogQueueName = attribute.Value;
            }

            return config;
        }

        public string ServiceNamespace { get; private set; }

        public string ServiceIssuer { get; private set; }

        public string ServiceSecret { get; private set; }

        public string QueueName { get; private set; }

        public string LogQueueName { get; private set; }
    }
}

