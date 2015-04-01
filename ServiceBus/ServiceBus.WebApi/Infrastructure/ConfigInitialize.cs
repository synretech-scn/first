using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.WebApi.Infrastructure
{
    public class ConfigInitialize
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Configuration.Config Initialize(bool forceRecreate)
        {
            if (Singleton<Configuration.Config>.Instance == null || forceRecreate)
            {
                Singleton<Configuration.Config>.Instance = ConfigurationManager.GetSection("Config") as Configuration.Config;
            }
            return Singleton<Configuration.Config>.Instance;
        }
    }
}
