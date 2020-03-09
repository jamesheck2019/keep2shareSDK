Imports keep2shareSDK.JSON
Imports Newtonsoft.Json

Public Class Authentication


#Region "Get_Token"
    Shared Async Function OneHourToken(Username As String, Password As String, Optional CaptchaWord As String = Nothing, Optional ChallengeID As String = Nothing) As Task(Of String)
        Net.ServicePointManager.Expect100Continue = True : Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Ssl3

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("login"))
            Dim JSONobj = New With {.username = Username, .password = Password, .captcha_challenge = ChallengeID, .captcha_response = CaptchaWord}
            HtpReqMessage.Content = JSONobj.JsonContent
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If Linq.JObject.Parse(result).SelectToken("status").ToString = "success" Then
                    Return Linq.JObject.Parse(result).SelectToken("auth_token").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

End Class
