Imports System

Public Class keep2shareException
    Inherits Exception

    Public Sub New(ByVal errorMessage As String, ByVal errorCode As String)
        MyBase.New(errorMessage)
    End Sub
End Class


Public Class ExceptionCls
    Public Shared Function CreateException(errorMesage As String, errorCode As String) As keep2shareException
        Return New keep2shareException(errorMesage, errorCode)
    End Function
End Class

