using keep2shareSDK.JSON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static KClient;

namespace keep2shareSDK
{
  public   interface IClient
    {

        Task<JSON_FileMetadata> FileMetadata(string DestinationFileID);
        Task<int> DeleteMultiple(List<string> DestinationFileFolderIDs);
        Task<bool> Delete(string DestinationFileFolderID);
        Task<JSON_Upload> Upload(object FileToUpload, Utilitiez.UploadTypes UploadType, string DestinationFolderID, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
        Task<JSON_ListFolders> ListFolders();
        Task<System.Drawing.Image> CaptchaImage(string CaptchaUrl);
        Task<JSON.JSON_RequestCaptcha> RequestCaptcha();
        Task<JSON_TestAccessToken> TestAccessToken();
        Task<JSON_AccountInfo> AccountInfo();
        Task<JSON_FilesList> RootList(LocationType FileorFolder, int Limit, int OffSet, RootListOptions.SortOptions SortBy, bool PublicFilesFoldersOnly = false, bool DetailedJson = false);
        Task<JSON_FilesList> ListFiles(string DestinationFolderID, LocationType FileorFolder, int Limit, int OffSet, RootListOptions.SortOptions SortBy = null, bool DetailedJson = false);



        Task<JSON_CreateNewFolder> CreateNewFolder(string DestinationFolderID, string FolderName, bool SetPublic);
        Task<bool> Rename(string DestinationFileFolderID, string NewName);
        Task<JSON_RemoteUpload> RemoteUpload(List<string> Links);
        Task<JSON_RemoteUploadStatus> RemoteUploadStatus(List<string> JobIDs);
        Task<bool> Move(string SourceFileFolderID, string DestinationFolderID);
        Task<bool> MoveAndRename(string SourceFileFolderID, string DestinationFolderID, string RenameTo);
        Task<bool> ChangePrivacy(string DestinationFileFolderID, Utilitiez.SetAccess Access);


    }
}
