Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.Json

Public Class NotPixelBot

    Private ReadOnly PubQuery As NotPixelQuery
    Private ReadOnly PubProxy As Proxy()
    Private ReadOnly Reward As Integer() = {5, 100, 200, 300, 500, 600}
    Private ReadOnly Speed As Integer() = {5, 100, 200, 300, 400, 500, 600, 700, 800, 900}
    Private ReadOnly Limit As Integer() = {5, 100, 200, 300, 400}
    Public ReadOnly HasError As Boolean
    Public ReadOnly ErrorMessage As String
    Public ReadOnly IPAddress As String

    Public Sub New(Query As NotPixelQuery, Proxy As Proxy())
        PubQuery = Query
        PubProxy = Proxy
        IPAddress = GetIP().Result
        PubQuery.Auth = getSession().Result
        Dim GetToken = NotPixelLoginAsync().Result
        If GetToken IsNot Nothing Then
            HasError = False
            ErrorMessage = ""
        Else
            HasError = True
            ErrorMessage = "login failed"
        End If
    End Sub

    Private Async Function GetIP() As Task(Of String)
        Dim client As HttpClient
        Dim FProxy = PubProxy.Where(Function(x) x.Index = PubQuery.Index)
        If FProxy.Count <> 0 Then
            If FProxy(0).Proxy <> "" Then
                Dim handler = New HttpClientHandler With {.Proxy = New WebProxy With {.Address = New Uri(FProxy(0).Proxy)}}
                client = New HttpClient(handler) With {.Timeout = New TimeSpan(0, 0, 30)}
            Else
                client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
            End If
        Else
            client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
        End If
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync($"https://httpbin.org/ip")
        Catch ex As Exception
        End Try
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of httpbin)(responseStream)
                Return responseJson.Origin
            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function

    Private Async Function getSession() As Task(Of String)
        Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "notpixel", "https://notpx.app/")
        Dim url As String = Await vw.Get_URL()
        If url <> "" Then
            Return url.Split(New String() {"tgWebAppData="}, StringSplitOptions.None)(1).Split(New String() {"&tgWebAppVersion"}, StringSplitOptions.None)(0)
        Else
            Return ""
        End If
    End Function

    Private Async Function NotPixelLoginAsync() As Task(Of NotPixelUserDetailResponse)
        Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/users/me")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelUserDetailResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function NotPixelSyncAsync() As Task(Of NotPixelStatusResponse)
        Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/mining/status")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelStatusResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function NotPixelClaimAsync() As Task(Of NotPixelStatusResponse)
        Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/mining/claim")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelStatusResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function NotPixelPaintAsync() As Task(Of NotPixelPaintResponse)
        Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim rnd As New Random
        Dim request As New NotPixelPaintRequest With {.PixelId = rnd.Next(1000, 999999), .NewColor = String.Format("#{0:X6}", rnd.Next(&H1000000)).ToUpper}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        'Dim serializedRequest = JsonSerializer.Serialize(Await GetCor())
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await NPAPI.NPAPIPost("https://notpx.app/api/v1/repaint/start", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelPaintResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function NotPixelUpgradeRewardAsync() As Task(Of Integer)
        Dim sync = Await NotPixelSyncAsync()
        If sync IsNot Nothing Then
            If sync.Boosts.PaintReward <= Reward.Count Then
                If sync.UserBalance > Reward(sync.Boosts.PaintReward - 1) Then
                    Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
                    Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/mining/boost/check/paintReward")
                    If httpResponse IsNot Nothing Then
                        If httpResponse.IsSuccessStatusCode Then
                            Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                            Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelPaintRewardResponse)(responseStream)
                            Return IIf(responseJson.PaintReward, 2, 0)
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                Else
                    Return 1
                End If
            Else
                Return 1
            End If
        Else
            Return 0
        End If
    End Function

    Public Async Function NotPixelUpgradeSpeedAsync() As Task(Of Integer)
        Dim sync = Await NotPixelSyncAsync()
        If sync IsNot Nothing Then
            If sync.Boosts.ReChargeSpeed <= Speed.Count Then
                If sync.UserBalance > Speed(sync.Boosts.ReChargeSpeed - 1) Then
                    Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
                    Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/mining/boost/check/reChargeSpeed")
                    If httpResponse IsNot Nothing Then
                        If httpResponse.IsSuccessStatusCode Then
                            Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                            Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelReChargeSpeedResponse)(responseStream)
                            Return IIf(responseJson.ReChargeSpeed, 2, 0)
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                Else
                    Return 1
                End If
            Else
                Return 1
            End If
        Else
            Return 0
        End If
    End Function

    Public Async Function NotPixelUpgradeLimitAsync() As Task(Of Integer)
        Dim sync = Await NotPixelSyncAsync()
        If sync IsNot Nothing Then
            If sync.Boosts.EnergyLimit <= Limit.Count Then
                If sync.UserBalance > Limit(sync.Boosts.EnergyLimit - 1) Then
                    Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
                    Dim httpResponse = Await NPAPI.NPAPIGet("https://notpx.app/api/v1/mining/boost/check/energyLimit")
                    If httpResponse IsNot Nothing Then
                        If httpResponse.IsSuccessStatusCode Then
                            Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                            Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelEnergyLimitResponse)(responseStream)
                            Return IIf(responseJson.EnergyLimit, 2, 0)
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                Else
                    Return 1
                End If
            Else
                Return 1
            End If
        Else
            Return 0
        End If
    End Function

    Private Async Function GetCor() As Task(Of NotPixelPaintRequest)
        Dim client As New HttpClient With {
            .Timeout = New TimeSpan(0, 0, 30)
        }
        client.DefaultRequestHeaders.CacheControl = New CacheControlHeaderValue With {.NoCache = True, .NoStore = True, .MaxAge = TimeSpan.FromSeconds(0)}
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync("https://raw.githubusercontent.com/glad-tidings/NotPixelBot/refs/heads/main/list.json")
        Catch ex As Exception
        End Try
        If httpResponse.IsSuccessStatusCode Then
            Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
            Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixel3XPoint)(responseStream)
            Dim rnd As New Random
            Dim paint = responseJson.Data(rnd.Next(responseJson.Data.Count()))
            Dim color = paint.Color
            Dim randomCor = paint.Cordinates(rnd.Next(paint.Cordinates.Count()))
            Dim pxId As Integer = CalcId(randomCor.Start(0), randomCor.Start(1), randomCor.End(0), randomCor.End(1))
            Return New NotPixelPaintRequest With {.PixelId = pxId, .NewColor = color}
        Else
            Return Nothing
        End If
    End Function

    Private Function CalcId(ByVal x As Integer, ByVal y As Integer, ByVal x1 As Integer, ByVal y1 As Integer) As Integer
        Dim rnd As New Random
        Dim pxId As Integer = rnd.Next(Math.Min(y, y1), Math.Max(y1, y) + 1) * 1000
        pxId += rnd.Next(Math.Min(x, x1), Math.Max(x1, x) + 1) + 1
        Return pxId
    End Function

    Public Async Function NotPixelGetSecretAsync() As Task(Of NotPixelGetSecretResponse)
        Dim client As New HttpClient With {
            .Timeout = New TimeSpan(0, 0, 30)
        }
        client.DefaultRequestHeaders.CacheControl = New CacheControlHeaderValue With {.NoCache = True, .NoStore = True, .MaxAge = TimeSpan.FromSeconds(0)}
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync("https://raw.githubusercontent.com/glad-tidings/NotPixelBot/refs/heads/main/secret.json")
        Catch ex As Exception
        End Try
        If httpResponse.IsSuccessStatusCode Then
            Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
            Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelGetSecretResponse)(responseStream)
            Return responseJson
        Else
            Return Nothing
        End If
    End Function

    Public Async Function NotPixelSecretAsync(secretWord As String) As Task(Of Boolean)
        Dim NPAPI As New NotPixelApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim rnd As New Random
        Dim request As New NotPixelSecretRequest With {.SecretWord = secretWord}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await NPAPI.NPAPIPost("https://notpx.app/api/v1/mining/quest/check/secretWord", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of NotPixelSecretResponse)(responseStream)
                Return responseJson.SecretWord.Success
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

End Class
