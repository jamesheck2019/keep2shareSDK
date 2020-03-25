using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using static keep2shareSDK.Basic;
using System.Net;
using System.Net.Http;


namespace keep2shareSDK
{
    public class Authentication
    {

        public static async Task<string> OneHourToken(string Username, string Password, string CaptchaWord = null, string ChallengeID = null)
        {
            ServicePointManager.Expect100Continue = true; ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("login"));
                var JSONobj = new { username = Username, password = Password, captcha_challenge = ChallengeID, captcha_response = CaptchaWord };
                HtpReqMessage.Content = JSONobj.JsonContent();
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();

                    if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                    {
                        return JObject.Parse(result).SelectToken("auth_token").ToString();
                    }
                    else
                    {
                        ShowError(result);
                        return null;
                    }
                }
            }
        }
    }


}
