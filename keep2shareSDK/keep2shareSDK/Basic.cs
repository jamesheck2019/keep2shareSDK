using keep2shareSDK.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace keep2shareSDK
{
    public static class Basic
    {

        public static string APIbase = "http://keep2share.cc/api/v2/";
        public static TimeSpan m_TimeOut = System.Threading.Timeout.InfiniteTimeSpan;
        public static bool m_CloseConnection = true;
        public static JsonSerializerSettings JSONhandler = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
        public static string authToken = null;


        private static ProxyConfig _proxy;
        public static ProxyConfig m_proxy
        {
            get
            {
                return _proxy ?? new ProxyConfig();
            }
            set
            {
                _proxy = value;
            }
        }


        public class HCHandler : System.Net.Http.HttpClientHandler
        {
            public HCHandler() : base()
            {
                if (m_proxy.SetProxy)
                {
                    base.MaxRequestContentBufferSize = 1 * 1024 * 1024;
                    base.Proxy = new System.Net.WebProxy($"http://{m_proxy.ProxyIP}:{m_proxy.ProxyPort}", true, null, new System.Net.NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword));
                    base.UseProxy = m_proxy.SetProxy;
                }
            }
        }

        public class HtpClient : System.Net.Http.HttpClient
        {
            public HtpClient(HCHandler HCHandler) : base(HCHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("keep2shareSDK");
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
            public HtpClient(System.Net.Http.Handlers.ProgressMessageHandler progressHandler) : base(progressHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("keep2shareSDK");
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
        }

        public class pUri : Uri
        {
            public pUri(string ApiAction) : base(APIbase + ApiAction) { }
        }

        public static keep2shareException ShowError(string result)
        {
            JSON_Error errorInfo = JsonConvert.DeserializeObject<JSON_Error>(result, JSONhandler);
            throw new keep2shareException(errorInfo.ErrorMessage, 1002);
        }

        public static HttpContent JsonContent(this object JsonCls)
        {
            return new StringContent(JsonConvert.SerializeObject(JsonCls, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json");
        }

        public static async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string url, HttpContent content)
        {
            using (HtpClient localHtpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage requ = new HttpRequestMessage(method, new Uri(APIbase + url)) { Content = content };
                HttpResponseMessage response = await localHtpClient.SendAsync(requ, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                return response;
            }
        }

        public static bool Success(this HttpResponseMessage Response)
        {
            return Response.StatusCode== System.Net.HttpStatusCode.OK && JObject.Parse(Response.Content.ReadAsStringAsync().Result).SelectToken("status").ToString() == "success" ? true: false ;
        }


    }
}
