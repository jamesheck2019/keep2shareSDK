using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using keep2shareSDK.JSON;

namespace keep2shareSDK
{
	public interface IClient
	{
		Task<JSON_FileMetadata> FileMetadata(string DestinationFileID);

		Task<int> DeleteMultiple(List<string> DestinationFileFolderIDs);

		Task<bool> Delete(string DestinationFileFolderID);

		Task<JSON_Upload> Upload(object FileToUpload, KClient.UploadTypes UploadType, string DestinationFolderID, string FileName, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, CancellationToken token = default(CancellationToken));

		Task<JSON_ListFolders> ListFolders();

		Task<Image> CaptchaImage(string CaptchaUrl);

		Task<JSON_RequestCaptcha> RequestCaptcha();

		Task<JSON_TestAccessToken> TestAccessToken();

		Task<JSON_AccountInfo> AccountInfo();

		Task<JSON_FilesList> RootList(KClient.LocationType FileorFolder, int Limit, int OffSet, KClient.RootListOptions.SortOptions SortBy, bool PublicFilesFoldersOnly = false, bool DetailedJson = false);

		Task<JSON_FilesList> ListFiles(string DestinationFolderID, KClient.LocationType FileorFolder, int Limit, int OffSet, KClient.RootListOptions.SortOptions SortBy = null, bool DetailedJson = false);

		Task<JSON_CreateNewFolder> CreateNewFolder(string DestinationFolderID, string FolderName, bool SetPublic);

		Task<bool> Rename(string DestinationFileFolderID, string NewName);

		Task<JSON_RemoteUpload> RemoteUpload(List<string> Links);

		Task<JSON_RemoteUploadStatus> RemoteUploadStatus(List<string> JobIDs);

		Task<bool> Move(string SourceFileFolderID, string DestinationFolderID);

		Task<bool> MoveAndRename(string SourceFileFolderID, string DestinationFolderID, string RenameTo);

		Task<bool> ChangeAccess(string DestinationFileFolderID, KClient.SetAccess Access);
	}
}
