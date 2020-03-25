using System;
using System.Collections.Generic;
using System.Text;
using keep2shareSDK;

namespace keep2shareSDK
{
    public class ConnectionSettings
    {
        public TimeSpan? TimeOut = null;
        public bool? CloseConnection = true;
        public ProxyConfig Proxy = null;
    }
}
