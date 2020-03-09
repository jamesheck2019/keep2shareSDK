## keep2shareSDK

`Download:`
[https://github.com/jamesheck2019/keep2shareSDK/releases](https://github.com/jamesheck2019/keep2shareSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.keep2shareSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.keep2shareSDK/)<br>
`Help:`
[https://github.com/jamesheck2019/keep2shareSDK/wiki](https://github.com/jamesheck2019/keep2shareSDK/wiki)<br>

# Features
* Assemblies for .NET 4.5.2 and .NET Standard 2.0 and .NET Core 2.1
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support

# List of functions:
* FileMetadata
* DeleteMultiple
* Delete
* Upload
* ListFolders
* CaptchaImage
* RequestCaptcha
* TestAccessToken
* AccountInfo
* RootList
* ListFiles
* CreateNewFolder
* Rename
* RemoteUpload
* RemoteUploadStatus
* Move
* MoveAndRename
* ChangePrivacy


# Example:
```vb.net

'first get auth token (one time only)
Dim tokn = keep2shareSDK.Authentication.OneHourToken("user", "pass")

''set proxy and connection options
Dim con As New keep2shareSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New keep2shareSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "127.0.0.1", .ProxyPort = 8888, .ProxyUsername = "user", .ProxyPassword = "pass"}}
''set api client
Dim CLNT As keep2shareSDK.IClient = New keep2shareSDK.KClient("xxxxxtokenxxxxx", con)

''general
Await CLNT.AccountInfo()
Dim cap = Await CLNT.RequestCaptcha()
Await CLNT.CaptchaImage(cap.captcha_url)
Await CLNT.TestAccessToken()

''file folder
Await CLNT.CreateNewFolder("root id = /", "new folder", True)
Dim cts As New Threading.CancellationTokenSource()
Dim _ReportCls As New Progress(Of keep2shareSDK.ReportStatus)(Sub(ReportClass As keep2shareSDK.ReportStatus) Console.WriteLine(String.Format("{0} - {1}% - {2}", String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes)), CInt(ReportClass.ProgressPercentage), ReportClass.TextStatus)))
Await CLNT.Upload("c:\\VIDO.mp4", UploadTypes.FilePath, "/", "VIDO.mp4", _ReportCls, cts.Token)
Await CLNT.Delete("id")
Await CLNT.DeleteMultiple(New List(Of String) From {{"id"}, {"id"}})
Await CLNT.FileMetadata("id")
Await CLNT.ListFiles("id", keep2shareSDK.KClient.LocationType.any, 50, 1, New keep2shareSDK.KClient.RootListOptions.SortOptions With {.name = keep2shareSDK.KClient.SoRt.ascending}, True)
Await CLNT.ListFolders
Await CLNT.Move("from id", "to id")
Await CLNT.MoveAndRename("from id", "to id", "rename to")
Await CLNT.RemoteUpload(New List(Of String) From {{"http://telerik.com/blob.mp4"}})
Await CLNT.RemoteUploadStatus(New List(Of String) From {{"jobid"}})
Await CLNT.Rename("id", "newname")
Await CLNT.RootList(keep2shareSDK.KClient.LocationType.folder, 50, 2, New keep2shareSDK.KClient.RootListOptions.SortOptions With {.name = keep2shareSDK.KClient.SoRt.ascending}, False, True)
   
```
