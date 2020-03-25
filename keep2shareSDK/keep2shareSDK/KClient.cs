using keep2shareSDK;
using keep2shareSDK.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static keep2shareSDK.Basic;
using static keep2shareSDK.Utilitiez;

public class KClient : IClient
{
    // 'root folder = "/"
    public KClient(string accessToken, ConnectionSettings Settings = null)
    {
        ServicePointManager.Expect100Continue = true; ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        authToken = accessToken;

        if (Settings == null)
        {
            m_proxy = null;
        }
        else
        {
            m_proxy = Settings.Proxy;
            m_CloseConnection = Settings.CloseConnection ?? true;
            m_TimeOut = Settings.TimeOut ?? TimeSpan.FromMinutes(60);
        }
    }


    #region useUnsafeHeaderParsing
    // ' solving exception message: "The server committed a protocol violation. Section=ResponseHeader Detail=CR must be followed by LF"
    // 'https://social.msdn.microsoft.com/Forums/en-US/ff098248-551c-4da9-8ba5-358a9f8ccc57/how-do-i-enable-useunsafeheaderparsing-from-code-net-20
    // Public Shared Function SetAllowUnsafeHeaderParsing20() As Boolean
    // Dim aNetAssembly As Reflection.Assembly = Reflection.Assembly.GetAssembly(GetType(System.Net.Configuration.SettingsSection))

    // If aNetAssembly IsNot Nothing Then
    // Dim aSettingsType As Type = aNetAssembly.[GetType]("System.Net.Configuration.SettingsSectionInternal")

    // If aSettingsType IsNot Nothing Then
    // Dim anInstance As Object = aSettingsType.InvokeMember("Section", Reflection.BindingFlags.[Static] Or Reflection.BindingFlags.GetProperty Or Reflection.BindingFlags.NonPublic, Nothing, Nothing, New Object() {})

    // If anInstance IsNot Nothing Then
    // Dim aUseUnsafeHeaderParsing As Reflection.FieldInfo = aSettingsType.GetField("useUnsafeHeaderParsing", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)

    // If aUseUnsafeHeaderParsing IsNot Nothing Then
    // aUseUnsafeHeaderParsing.SetValue(anInstance, True)
    // Return True
    // End If
    // End If
    // End If
    // End If

    // Return False
    // End Function
    #endregion

    #region RequestCaptcha
    public async Task<JSON_RequestCaptcha> RequestCaptcha()
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("RequestCaptcha"));
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                var userInfo = JsonConvert.DeserializeObject<JSON_RequestCaptcha>(result, JSONhandler);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var img = await CaptchaImage(userInfo.captcha_url);
                    userInfo.captcha_image = img;
                    return userInfo;
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region CaptchaImage
    public async Task<System.Drawing.Image> CaptchaImage(string CaptchaUrl)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(CaptchaUrl));
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                var _stream = await response.Content.ReadAsStreamAsync();
                System.Drawing.Image image = System.Drawing.Image.FromStream(_stream);
                return image;
            }
        }
    }
    #endregion

    #region TestAccessToken
    public async Task<JSON_TestAccessToken> TestAccessToken()
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("test"));
            HtpReqMessage.Content = new StringContent(JsonConvert.SerializeObject(new { auth_token = authToken }), System.Text.Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_TestAccessToken>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region AccountInfo
    public async Task<JSON_AccountInfo> AccountInfo()
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("AccountInfo"));
            HtpReqMessage.Content = (new { auth_token = authToken }).JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_AccountInfo>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region RootList
    public enum SoRt { descending = -1, ascending = 1 }
    public enum LocationType { any, file, folder }
    public class RootListOptions
    {
        public string auth_token { get; set; }
        public string parent { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public SortOptions sort { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public LocationType? type { get; set; }
        public bool only_available { get; set; }
        public bool extended_info { get; set; }
        public class SortOptions
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public SoRt? id { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public SoRt? name { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public SoRt? date_created { get; set; }
        }
    }
    public async Task<JSON_FilesList> RootList(LocationType FileorFolder, int Limit, int OffSet, RootListOptions.SortOptions SortBy, bool PublicFilesFoldersOnly = false, bool DetailedJson = false)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("GetFilesList"));
            var JSONobj = new RootListOptions() { auth_token = authToken, parent = "/", limit = Limit, offset = OffSet, sort = SortBy, type = FileorFolder, only_available = PublicFilesFoldersOnly, extended_info = DetailedJson };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_FilesList>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region ListFiles
    public async Task<JSON_FilesList> ListFiles(string DestinationFolderID, LocationType FileorFolder, int Limit, int OffSet, RootListOptions.SortOptions SortBy = null, bool DetailedJson = false)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("GetFilesList"));
            var JSONobj = new RootListOptions() { auth_token = authToken, parent = DestinationFolderID, limit = Limit, offset = OffSet, sort = SortBy, type = FileorFolder, only_available = false, extended_info = DetailedJson };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_FilesList>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region ListFolders
    public async Task<JSON_ListFolders> ListFolders()
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("GetFoldersList"));
            HtpReqMessage.Content = (new { auth_token = authToken }).JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_ListFolders>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region CreateNewFolder
    public async Task<JSON_CreateNewFolder> CreateNewFolder(string DestinationFolderID, string FolderName, bool SetPublic)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("CreateFolder"));
            var JSONobj = new { auth_token = authToken, parent = DestinationFolderID, name = FolderName, is_public = SetPublic, access = SetAccess.@public.ToString() };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_CreateNewFolder>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region RenameFileFolder
    public async Task<bool> Rename(string DestinationFileFolderID, string NewName)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("UpdateFile"));
            var JSONobj = new { auth_token = authToken, id = DestinationFileFolderID, new_name = NewName };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return true;
                }
                else
                {
                    ShowError(result);
                    return false;
                }
            }
        }
    }
    #endregion

    #region Move
    public async Task<bool> Move(string SourceFileFolderID, string DestinationFolderID)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("UpdateFile"));
            var JSONobj = new { auth_token = authToken, id = SourceFileFolderID, new_parent = DestinationFolderID };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return true;
                }
                else
                {
                    ShowError(result);
                    return false;
                }
            }
        }
    }
    #endregion

    #region MoveAndRename
    public async Task<bool> MoveAndRename(string SourceFileFolderID, string DestinationFolderID, string RenameTo)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("UpdateFile"));
            var JSONobj = new { auth_token = authToken, id = SourceFileFolderID, new_parent = DestinationFolderID, new_name = RenameTo };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return true;
                }
                else
                {
                    ShowError(result);
                    return false;
                }
            }
        }
    }
    #endregion

    #region ChangeFileFolderAccess
    private class ChangeFileFolderAccessOptions
    {
        public string auth_token { get; set; }
        public string id { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] public string new_access { get; set; }
    }
    public async Task<bool> ChangePrivacy(string DestinationFileFolderID, SetAccess Access)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("UpdateFile"));
            var JSONobj = new ChangeFileFolderAccessOptions() { auth_token = authToken, id = DestinationFileFolderID, new_access = Access.ToString() };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return true;
                }
                else
                {
                    ShowError(result);
                    return false;
                }
            }
        }
    }
    #endregion

    #region RemoteUpload
    public async Task<JSON_RemoteUpload> RemoteUpload(List<string> Links)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("RemoteUploadAdd"));
            HtpReqMessage.Content = (new { auth_token = authToken, urls = Links }).JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_RemoteUpload>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region RemoteUploadStatus
    public async Task<JSON_RemoteUploadStatus> RemoteUploadStatus(List<string> JobIDs)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("RemoteUploadStatus"));
            HtpReqMessage.Content = (new { auth_token = authToken, ids = JobIDs }).JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_RemoteUploadStatus>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region Upload
    private async Task<JSON_GetUploadUrl> GetUploadUrl(string DestinationFolderID)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("GetUploadFormData"));
            var JSONobj = new { auth_token = authToken, parent_id = DestinationFolderID, preferred_node = string.Empty };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_GetUploadUrl>(result, JSONhandler);
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }

    public async Task<JSON_Upload> Upload(object FileToUpload, UploadTypes UploadType, string DestinationFolderID, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
    {
        var uploadUrl = await GetUploadUrl(DestinationFolderID);

        ReportCls = ReportCls ?? new Progress<ReportStatus>();
        ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
        try
        {
            System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
            progressHandler.HttpSendProgress += (sender, e) => { ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Uploading..." }); };
            HttpClient localHttpClient = new HtpClient(progressHandler);
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(uploadUrl.form_action));
            MultipartFormDataContent MultipartsformData = new MultipartFormDataContent();
            HttpContent streamContent=null;
            switch (UploadType)
            {
                case  UploadTypes.FilePath:
                        streamContent = new StreamContent(new System.IO.FileStream(FileToUpload.ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read));
                        break;
                case  UploadTypes.Stream:
                        streamContent = new StreamContent((System.IO.Stream)FileToUpload);
                        break;
                case  UploadTypes.BytesArry:
                        streamContent = new StreamContent(new System.IO.MemoryStream((byte[])FileToUpload));
                        break;
            }
            streamContent.Headers.Clear();
            streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = FileName };
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            MultipartsformData.Add(streamContent);
            // '''''''''''''''''''''''''
            MultipartsformData.Add(new StringContent(uploadUrl.form_data.ajax.ToString()), "ajax");
            MultipartsformData.Add(new StringContent(uploadUrl.form_data.@params), "params");
            MultipartsformData.Add(new StringContent(uploadUrl.form_data.signature), "signature");
            HtpReqMessage.Content = MultipartsformData;
            // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            using (HttpResponseMessage ResPonse = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
            {
                string result = await ResPonse.Content.ReadAsStringAsync();

                token.ThrowIfCancellationRequested();
                ResPonse.EnsureSuccessStatusCode();
                var userInfo = JsonConvert.DeserializeObject<JSON_Upload>(result, JSONhandler);
                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = $"[{FileName}] Uploaded successfully" });
                    return userInfo;
                }
                else
                {
                    var errorInfo = JsonConvert.DeserializeObject<JSON_Error>(result, JSONhandler);
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = $"The request returned with HTTP status code {errorInfo.ErrorMessage}" });
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            ReportCls.Report(new ReportStatus() { Finished = true });
            if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
            {
                ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
            }
            else
            {
                throw new keep2shareException (ex.Message, 1001);
            }
            return null;
        }
    }
    #endregion

    #region FileMetadata
    public async Task<JSON_FileMetadata> FileMetadata(string DestinationFileID)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("GetFilesInfo"));
            var JSONobj = new { auth_token = authToken, ids = new List<string>() { { DestinationFileID } }, extended_info = true };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return JsonConvert.DeserializeObject<JSON_FilesList>(result, JSONhandler).FilesList[0];
                }
                else
                {
                    ShowError(result);
                    return null;
                }
            }
        }
    }
    #endregion

    #region DeleteMultiple
    public async Task<int> DeleteMultiple(List<string> DestinationFileFolderIDs)
    {
        using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
        {
            HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("DeleteFiles"));
            var JSONobj = new { auth_token = authToken, ids = DestinationFileFolderIDs };
            HtpReqMessage.Content = JSONobj.JsonContent();
            using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();

                if (JObject.Parse(result).SelectToken("status").ToString() == "success")
                {
                    return Convert.ToInt32(JObject.Parse(result).SelectToken("deleted").ToString());
                }
                else
                {
                    ShowError(result);
                    return 0;
                }
            }
        }
    }
    #endregion

    #region Delete
    public async Task<bool> Delete(string DestinationFileFolderID)
    {
        var strt = await DeleteMultiple(new List<string>() { { DestinationFileFolderID } });
        return strt.Equals(1) ? true : false;
    }
    #endregion


}


