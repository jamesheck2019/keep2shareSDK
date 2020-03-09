
Public Class utilitiez

    Enum SetAccess
        [public]
        [Private]
        premium
    End Enum

    Enum UploadTypes
        FilePath
        Stream
        BytesArry
    End Enum
End Class

Public Class ConnectionSettings
    Public Property TimeOut As System.TimeSpan = Nothing
    Public Property CloseConnection As Boolean? = True
    Public Property Proxy As ProxyConfig = Nothing
End Class