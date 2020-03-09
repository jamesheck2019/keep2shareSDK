Imports keep2shareSDK.JSON
Imports keep2shareSDK.KClient

Public Interface IClient

    Function FileMetadata(DestinationFileID As String) As Task(Of JSON_FileMetadata)
    Function DeleteMultiple(DestinationFileFolderIDs As List(Of String)) As Task(Of Integer)
    Function Delete(DestinationFileFolderID As String) As Task(Of Boolean)
    Function Upload(FileToUpload As Object, UploadType As utilitiez.UploadTypes, DestinationFolderID As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_Upload)
    Function ListFolders() As Task(Of JSON_ListFolders)
    Function CaptchaImage(CaptchaUrl As String) As Task(Of System.Drawing.Image)
    Function RequestCaptcha() As Task(Of JSON.JSON_RequestCaptcha)
    Function TestAccessToken() As Task(Of JSON_TestAccessToken)
    Function AccountInfo() As Task(Of JSON_AccountInfo)
    Function RootList(FileorFolder As LocationType, Limit As Integer, OffSet As Integer, SortBy As RootListOptions.SortOptions, Optional PublicFilesFoldersOnly As Boolean = False, Optional DetailedJson As Boolean = False) As Task(Of JSON_FilesList)
    Function ListFiles(DestinationFolderID As String, FileorFolder As LocationType, Limit As Integer, OffSet As Integer, Optional SortBy As RootListOptions.SortOptions = Nothing, Optional DetailedJson As Boolean = False) As Task(Of JSON_FilesList)
    Function CreateNewFolder(DestinationFolderID As String, FolderName As String, SetPublic As Boolean) As Task(Of JSON_CreateNewFolder)
    Function Rename(DestinationFileFolderID As String, NewName As String) As Task(Of Boolean)
    Function RemoteUpload(Links As List(Of String)) As Task(Of JSON_RemoteUpload)
    Function RemoteUploadStatus(JobIDs As List(Of String)) As Task(Of JSON_RemoteUploadStatus)
    Function Move(SourceFileFolderID As String, DestinationFolderID As String) As Task(Of Boolean)
    Function MoveAndRename(SourceFileFolderID As String, DestinationFolderID As String, RenameTo As String) As Task(Of Boolean)
    Function ChangePrivacy(DestinationFileFolderID As String, Access As utilitiez.SetAccess) As Task(Of Boolean)


End Interface
