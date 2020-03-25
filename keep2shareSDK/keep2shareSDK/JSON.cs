using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace keep2shareSDK.JSON
{

    #region JSON_Error
    public class JSON_Error
    {
        [JsonProperty("message")]public string ErrorMessage { get; set; }
    }
    #endregion

    #region JSON_RequestCaptcha
    public class JSON_RequestCaptcha
    {
        [JsonProperty("challenge")]
        public string ChallengeID { get; set; }
        private string _captcha_url;
        public string captcha_url
        {
            get
            {
                return System.Net.WebUtility.UrlDecode(_captcha_url);
            }
            set
            {
                _captcha_url = value;
            }
        }
        public System.Drawing.Image captcha_image { get; set; }
    }
    #endregion

    #region JSON_TestAccessToken
    public class JSON_TestAccessToken
    {
        public string status { get; set; }
        public int code { get; set; }
        public bool Success
        {
            get
            {
                return status == "success" ? true : false;
            }
        }
    }
    #endregion

    #region JSON_AccountInfo
    public class JSON_AccountInfo
    {
        public long available_traffic { get; set; }
        public bool account_expires { get; set; }
    }
    #endregion

    #region JSON_FilesList
    public class JSON_FilesList
    {
        [JsonProperty("files")]
        public List<JSON_FileMetadata> FilesList { get; set; }
    }
    #endregion

    #region JSON_FileMetadata
    public class JSON_FileMetadata
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool is_available { get; set; }
        public bool is_folder { get; set; }
        public string date_created { get; set; }
        public string access { get; set; }
        public string size { get; set; }
        public string md5 { get; set; }
        public JSON_FileMetadataExtendedInfo extended_info { get; set; }
      public   enum file_OR_folder
        {
            File,
            Folder
        }
        public file_OR_folder FileORFolder
        {
            get
            {
                return is_folder ? file_OR_folder.Folder : file_OR_folder.File;
            }
        }
    }
    public class JSON_FileMetadataExtendedInfo
    {
        public string storage_object { get; set; }
        public string size { get; set; }
        public string date_download_last { get; set; }
        public string downloads { get; set; }
        public string access { get; set; }
        public string content_type { get; set; }
    }
    #endregion

    #region JSON_CreateNewFolder
    public class JSON_CreateNewFolder
    {
        public string id { get; set; }
    }
    #endregion

    #region JSON_RemoteUpload
    public class JSON_RemoteUpload
    {
        public List<Acceptedurl> acceptedUrls { get; set; }
        public List<Rejectedurl> rejectedUrls { get; set; }
    }
    public class Acceptedurl
    {
        public string url { get; set; }
        [JsonProperty("id")]
        public string JobID { get; set; }
    }
    public class Rejectedurl
    {
        public string url { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region JSON_RemoteUploadStatus
    public class JSON_RemoteUploadStatus
    {
        public string status { get; set; }
        public int code { get; set; }
        public bool Success
        {
            get
            {
                return status == "success" ? true : false;
            }
        }
        [JsonExtensionData]
        public Dictionary<string, object> uploads { get; set; }
    }
    #endregion

    #region Uploads
    public class Uploads
    {
        public _11522884 _11522884 { get; set; }
        public _11522888 _11522888 { get; set; }
    }

    public class _11522884
    {
        public string status { get; set; }
        public string progress { get; set; }
    }

    public class _11522888
    {
        public string status { get; set; }
        public string progress { get; set; }
    }
    #endregion

    #region JSON_GetUploadUrl
    public class JSON_GetUploadUrl
    {
        public string form_action { get; set; }
        public string file_field { get; set; }
        public JSON_GetUploadUrlFormData form_data { get; set; }
    }
    public class JSON_GetUploadUrlFormData
    {
        public bool ajax { get; set; }
        public string @params { get; set; }
        public string signature { get; set; }
    }

    #endregion

    #region JSON_Upload
    public class JSON_Upload
    {
        public int status_code { get; set; }
        public bool success { get; set; }
        public string user_file_id { get; set; }
        public string link { get; set; }
    }
    #endregion

    public class JSON_ListFolders
    {
        public List<string> foldersList { get; set; }
    }


}
