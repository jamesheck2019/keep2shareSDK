Imports Newtonsoft.Json

Namespace JSON


    Public Class JSON_Error
         <JsonProperty("message", NullValueHandling:=NullValueHandling.Ignore)> Public Property ErrorMessage As String
    End Class

#Region "RequestCaptcha"
    Public Class JSON_RequestCaptcha
        <JsonProperty("challenge")> Public Property ChallengeID As String
        Private _captcha_url As String
        Public Property captcha_url As String
            Get
                Return Net.WebUtility.UrlDecode(_captcha_url)
            End Get
            Set(value As String)
                _captcha_url = value
            End Set
        End Property
        Public Property captcha_image As Drawing.Image
    End Class
#End Region

#Region "TestAccessToken"
    Public Class JSON_TestAccessToken
        Public Property status As String
        Public Property code As Integer
        Public ReadOnly Property Success As Boolean
            Get
                Return If(status = "success", True, False)
            End Get
        End Property
    End Class
#End Region

#Region "AccountInfo"
    Public Class JSON_AccountInfo
        Public Property available_traffic As Long
        Public Property account_expires As Boolean
    End Class
#End Region

#Region "FilesList"
    Public Class JSON_FilesList
        <JsonProperty("files")> Public Property FilesList As List(Of JSON_FileMetadata)
    End Class
    Public Class JSON_FileMetadata
        Public Property id As String
        Public Property name As String
        Public Property is_available As Boolean
        Public Property is_folder As Boolean
        Public Property date_created As String
        Public Property access As String
        Public Property size As String
        Public Property md5 As String
        Public Property extended_info As JSON_FileMetadataExtendedInfo
        Enum file_OR_folder
            File
            Folder
        End Enum
        Public ReadOnly Property FileORFolder As file_OR_folder
            Get
                Return If(is_folder, file_OR_folder.Folder, file_OR_folder.File)
            End Get
        End Property
    End Class
    Public Class JSON_FileMetadataExtendedInfo
        Public Property storage_object As String
        Public Property size As String
        Public Property date_download_last As String
        Public Property downloads As String
        Public Property access As String
        Public Property content_type As String
    End Class
#End Region

#Region "CreateNewFolder"
    Public Class JSON_CreateNewFolder
        Public Property id As String
    End Class
#End Region

#Region "RemoteUpload"
    Public Class JSON_RemoteUpload
        Public Property acceptedUrls As List(Of Acceptedurl)
        Public Property rejectedUrls As List(Of Rejectedurl)
    End Class
    Public Class Acceptedurl
        Public Property url As String
        <JsonProperty("id")> Public Property JobID As String
    End Class
    Public Class Rejectedurl
        Public Property url As String
        Public Property message As String
    End Class
#End Region

#Region "JSON_RemoteUploadStatus"
    Public Class JSON_RemoteUploadStatus
        Public Property status As String
        Public Property code As Integer
        Public ReadOnly Property Success As Boolean
            Get
                Return If(status = "success", True, False)
            End Get
        End Property
        <JsonExtensionData> Public Property uploads As Dictionary(Of String, Object)
    End Class

    Public Class Uploads
        Public Property _11522884 As _11522884
        Public Property _11522888 As _11522888
    End Class

    Public Class _11522884
        Public Property status As String
        Public Property progress As String
    End Class

    Public Class _11522888
        Public Property status As String
        Public Property progress As String
    End Class
#End Region

#Region "JSON_GetUploadUrl"
    Public Class JSON_GetUploadUrl
        Public Property form_action As String
        Public Property file_field As String
        Public Property form_data As JSON_GetUploadUrlFormData
    End Class
    Public Class JSON_GetUploadUrlFormData
        Public Property ajax As Boolean
        Public Property params As String
        Public Property signature As String
    End Class
#End Region

#Region "JSON_Upload"
    Public Class JSON_Upload
        Public Property status_code As Integer
        Public Property success As Boolean
        Public Property user_file_id As String
        Public Property link As String
    End Class
#End Region

#Region "JSON_ListFolders"
    Public Class JSON_ListFolders
        Public Property foldersList As List(Of String)
    End Class
#End Region


End Namespace

