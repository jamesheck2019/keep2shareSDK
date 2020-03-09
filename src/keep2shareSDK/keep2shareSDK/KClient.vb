Imports keep2shareSDK.JSON
Imports Newtonsoft.Json
Imports System.Configuration
Imports keep2shareSDK.utilitiez

''root folder = "/"


Public Class KClient
    Implements IClient

    Public Sub New(accessToken As String, Optional Settings As ConnectionSettings = Nothing)
        Net.ServicePointManager.Expect100Continue = True : Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Ssl3
        'SetAllowUnsafeHeaderParsing20() 'fixing
        If Settings Is Nothing Then
            m_proxy = Nothing
        Else
            m_proxy = Settings.Proxy
            m_CloseConnection = If(Settings.CloseConnection, True)
            m_TimeOut = If(Settings.TimeOut = Nothing, TimeSpan.FromMinutes(60), Settings.TimeOut)
        End If
        authToken = accessToken
    End Sub


#Region "useUnsafeHeaderParsing"
    '' solving exception message: "The server committed a protocol violation. Section=ResponseHeader Detail=CR must be followed by LF"
    ''https://social.msdn.microsoft.com/Forums/en-US/ff098248-551c-4da9-8ba5-358a9f8ccc57/how-do-i-enable-useunsafeheaderparsing-from-code-net-20
    'Public Shared Function SetAllowUnsafeHeaderParsing20() As Boolean
    '    Dim aNetAssembly As Reflection.Assembly = Reflection.Assembly.GetAssembly(GetType(System.Net.Configuration.SettingsSection))

    '    If aNetAssembly IsNot Nothing Then
    '        Dim aSettingsType As Type = aNetAssembly.[GetType]("System.Net.Configuration.SettingsSectionInternal")

    '        If aSettingsType IsNot Nothing Then
    '            Dim anInstance As Object = aSettingsType.InvokeMember("Section", Reflection.BindingFlags.[Static] Or Reflection.BindingFlags.GetProperty Or Reflection.BindingFlags.NonPublic, Nothing, Nothing, New Object() {})

    '            If anInstance IsNot Nothing Then
    '                Dim aUseUnsafeHeaderParsing As Reflection.FieldInfo = aSettingsType.GetField("useUnsafeHeaderParsing", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)

    '                If aUseUnsafeHeaderParsing IsNot Nothing Then
    '                    aUseUnsafeHeaderParsing.SetValue(anInstance, True)
    '                    Return True
    '                End If
    '            End If
    '        End If
    '    End If

    '    Return False
    'End Function
#End Region

#Region "RequestCaptcha"
    Public Async Function Get_RequestCaptcha() As Task(Of JSON.JSON_RequestCaptcha) Implements IClient.RequestCaptcha
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("RequestCaptcha"))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                Dim userInfo = JsonConvert.DeserializeObject(Of JSON.JSON_RequestCaptcha)(result, JSONhandler)
                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Dim img = Await Get_CaptchaImage(userInfo.captcha_url)
                    userInfo.captcha_image = img
                    Return userInfo
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CaptchaImage"
    Public Async Function Get_CaptchaImage(CaptchaUrl As String) As Task(Of System.Drawing.Image) Implements IClient.CaptchaImage
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New Uri(CaptchaUrl))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim _stream = Await response.Content.ReadAsStreamAsync()
                Dim image As Drawing.Image = System.Drawing.Image.FromStream(_stream)
                Return image
            End Using
        End Using
    End Function
#End Region

#Region "TestAccessToken"
    Public Async Function POST_AccessToken() As Task(Of JSON_TestAccessToken) Implements IClient.TestAccessToken
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage() With {.Method = Net.Http.HttpMethod.Post, .RequestUri = New pUri("test")}
            HtpReqMessage.Content = New Net.Http.StringContent(JsonConvert.SerializeObject(New With {.auth_token = authToken}), Text.Encoding.UTF8, "application/json")

            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_TestAccessToken)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "AccountInfo"
    Public Async Function GET_AccountInfo() As Task(Of JSON_AccountInfo) Implements IClient.AccountInfo
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("AccountInfo"))
            HtpReqMessage.Content = (New With {.auth_token = authToken}).JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_AccountInfo)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "RootList"
    Enum SoRt
        descending = -1
        ascending = 1
    End Enum
    Enum LocationType
        any
        file
        folder
    End Enum
    Public Class RootListOptions
        Public Property auth_token As String
        Public Property parent As String
        Public Property limit As String
        Public Property offset As String
        Public Property sort As SortOptions
        <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)> <JsonConverter(GetType(Converters.StringEnumConverter))>
        Public Property type As LocationType?
        Public Property only_available As Boolean
        Public Property extended_info As Boolean
        Public Class SortOptions
            <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)> Public Property id As SoRt?
            <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)> Public Property name As SoRt?
            <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)> Public Property date_created As SoRt?
        End Class
    End Class
    Public Async Function Get_RootList(FileorFolder As LocationType, Limit As Integer, OffSet As Integer, SortBy As RootListOptions.SortOptions, Optional PublicFilesFoldersOnly As Boolean = False, Optional DetailedJson As Boolean = False) As Task(Of JSON_FilesList) Implements IClient.RootList
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("GetFilesList"))
            Dim JSONobj = New RootListOptions With {.auth_token = authToken, .parent = "/", .limit = Limit, .offset = OffSet, .sort = SortBy, .type = FileorFolder, .only_available = PublicFilesFoldersOnly, .extended_info = DetailedJson}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_FilesList)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using

        End Using
    End Function
#End Region

#Region "ListFiles"
    Public Async Function Get_ListFiles(DestinationFolderID As String, FileorFolder As LocationType, Limit As Integer, OffSet As Integer, Optional SortBy As RootListOptions.SortOptions = Nothing, Optional DetailedJson As Boolean = False) As Task(Of JSON_FilesList) Implements IClient.ListFiles
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("GetFilesList"))
            Dim JSONobj = New RootListOptions With {.auth_token = authToken, .parent = DestinationFolderID, .limit = Limit, .offset = OffSet, .sort = SortBy, .type = FileorFolder, .only_available = False, .extended_info = DetailedJson}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_FilesList)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFolders"
    Public Async Function Get_ListFolders() As Task(Of JSON_ListFolders) Implements IClient.ListFolders
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("GetFoldersList"))
            HtpReqMessage.Content = (New With {.auth_token = authToken}).JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_ListFolders)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CreateNewFolder"
    Public Async Function GET_CreateNewFolder(DestinationFolderID As String, FolderName As String, SetPublic As Boolean) As Task(Of JSON_CreateNewFolder) Implements IClient.CreateNewFolder
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("CreateFolder"))
            Dim JSONobj = New With {.auth_token = authToken, .parent = DestinationFolderID, .name = FolderName, .is_public = SetPublic, .access = SetAccess.public.ToString}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_CreateNewFolder)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Rename"
    Public Async Function GET_RenameFileFolder(DestinationFileFolderID As String, NewName As String) As Task(Of Boolean) Implements IClient.Rename
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("UpdateFile"))
            Dim JSONobj = New With {.auth_token = authToken, .id = DestinationFileFolderID, .new_name = NewName}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Move"
    Public Async Function GET_Move(SourceFileFolderID As String, DestinationFolderID As String) As Task(Of Boolean) Implements IClient.Move
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("UpdateFile"))
            Dim JSONobj = New With {.auth_token = authToken, .id = SourceFileFolderID, .new_parent = DestinationFolderID}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "MoveAndRename"
    Public Async Function GET_MoveAndRename(SourceFileFolderID As String, DestinationFolderID As String, RenameTo As String) As Task(Of Boolean) Implements IClient.MoveAndRename
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("UpdateFile"))
            Dim JSONobj = New With {.auth_token = authToken, .id = SourceFileFolderID, .new_parent = DestinationFolderID, .new_name = RenameTo}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ChangeFileFolderAccess"
    Private Class ChangeFileFolderAccessOptions
        Public Property auth_token As String
        Public Property id As String
        <JsonConverter(GetType(Converters.StringEnumConverter))> Public Property new_access As String
    End Class
    Public Async Function _ChangePrivacy(DestinationFileFolderID As String, Access As SetAccess) As Task(Of Boolean) Implements IClient.ChangePrivacy
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("UpdateFile"))
            Dim JSONobj = New ChangeFileFolderAccessOptions With {.auth_token = authToken, .id = DestinationFileFolderID, .new_access = Access}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "RemoteUpload"
    Public Async Function POST_RemoteUpload(Links As List(Of String)) As Task(Of JSON_RemoteUpload) Implements IClient.RemoteUpload
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("RemoteUploadAdd"))
            HtpReqMessage.Content = (New With {.auth_token = authToken, .urls = Links}).JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_RemoteUpload)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "RemoteUploadStatus"
    Public Async Function POST_RemoteUploadStatus(JobIDs As List(Of String)) As Task(Of JSON_RemoteUploadStatus) Implements IClient.RemoteUploadStatus
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("RemoteUploadStatus"))
            HtpReqMessage.Content = (New With {.auth_token = authToken, .ids = JobIDs}).JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return JsonConvert.DeserializeObject(Of JSON_RemoteUploadStatus)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "GetUploadUrl"
    Private Async Function POST_GetUploadUrl(DestinationFolderID As String) As Task(Of JSON_GetUploadUrl)
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("GetUploadFormData"))
            Dim JSONobj = New With {.auth_token = authToken, .parent_id = DestinationFolderID, .preferred_node = Nothing}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_GetUploadUrl)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "UploadFile"
    Public Async Function Get_UploadLocal(FileToUpload As Object, UploadType As UploadTypes, DestinationFolderID As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_Upload) Implements IClient.Upload
        Dim uploadUrl = Await POST_GetUploadUrl(DestinationFolderID)

        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpSendProgress, (Function(sender, e)
                                                              ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Uploading..."})
                                                          End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New Uri(uploadUrl.form_action))
            Dim MultipartsformData As New Net.Http.MultipartFormDataContent()
            Dim streamContent As Net.Http.HttpContent
            Select Case UploadType
                Case UploadTypes.FilePath
                    streamContent = New Net.Http.StreamContent(New IO.FileStream(FileToUpload, IO.FileMode.Open, IO.FileAccess.Read))
                Case UploadTypes.Stream
                    streamContent = New Net.Http.StreamContent(CType(FileToUpload, IO.Stream))
                Case UploadTypes.BytesArry
                    streamContent = New Net.Http.StreamContent(New IO.MemoryStream(CType(FileToUpload, Byte())))
            End Select
            streamContent.Headers.Clear()
            streamContent.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") With {.Name = "file", .FileName = FileName}
            streamContent.Headers.ContentType = New Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream")
            MultipartsformData.Add(streamContent)
            ''''''''''''''''''''''''''
            MultipartsformData.Add(New Net.Http.StringContent(uploadUrl.form_data.ajax), "ajax")
            MultipartsformData.Add(New Net.Http.StringContent(uploadUrl.form_data.params), "params")
            MultipartsformData.Add(New Net.Http.StringContent(uploadUrl.form_data.signature), "signature")
            HtpReqMessage.Content = MultipartsformData
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(False)
                Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

                token.ThrowIfCancellationRequested()
                ResPonse.EnsureSuccessStatusCode()
                Dim userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(Of JSON_Upload)(result, JSONhandler)
                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("[{0}] Uploaded successfully", FileName)})
                    Return userInfo
                Else
                    Dim errorInfo = JsonConvert.DeserializeObject(Of JSON_Error)(result, JSONhandler)
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("The request returned with HTTP status code {0}", errorInfo.ErrorMessage)})
                End If
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "FileMetadata"
    Public Async Function GET_FileMetadata(DestinationFileID As String) As Task(Of JSON_FileMetadata) Implements IClient.FileMetadata
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("GetFilesInfo"))
            Dim JSONobj = New With {.auth_token = authToken, .ids = New List(Of String) From {{DestinationFileID}}, .extended_info = True}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                Return JsonConvert.DeserializeObject(Of JSON_FilesList)(result, JSONhandler).FilesList(0)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "DeleteMultiple"
    Public Async Function GET_DeleteMultiple(DestinationFileFolderIDs As List(Of String)) As Task(Of Integer) Implements IClient.DeleteMultiple
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("DeleteFiles"))
            Dim JSONobj = New With {.auth_token = authToken, .ids = DestinationFileFolderIDs}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return Linq.JObject.Parse(result).SelectToken("deleted").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Delete"
    Public Async Function GET_Delete(DestinationFileFolderID As String) As Task(Of Boolean) Implements IClient.Delete
        Dim strt = Await GET_DeleteMultiple(New List(Of String) From {{DestinationFileFolderID}})
        Return If(strt.Equals(1), True, False)
    End Function
#End Region


End Class

