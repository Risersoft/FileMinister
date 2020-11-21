Imports System.Net.Http
'Imports FileMinister.Models.Sync
Imports FileMinister.Models.Enums
Imports Newtonsoft.Json
'Imports risersoft.shared.portable
Imports System.Text
Imports System.Net.Http.Headers

Public MustInherit Class ProxyBase
    Implements IDisposable
    Dim path As String
    Dim timeout As TimeSpan = TimeSpan.FromSeconds(100)
    Dim baseAddress As String
    Dim accessToken As String

    Public Property UserEmail As String
    Public Property RequestTimeout As TimeSpan
        Get
            Return timeout
        End Get
        Set(value As TimeSpan)
            timeout = value
        End Set
    End Property


    Public Sub New(baseAddress As String, path As String, userEmail As String, accessToken As String)
        Me.baseAddress = baseAddress
        Me.path = path
        Me.UserEmail = userEmail
        Me.accessToken = accessToken
    End Sub

#Region "Async Methods"

    Public Async Function GetAsync(Of T)(Optional endpoint As String = "", Optional id As Object = Nothing, Optional queryString As IDictionary(Of String, Object) = Nothing) As Task(Of ResultInfo(Of T, Status))
        Try
            Using client = GetHttpClient()
                Dim uri As String = ConstructUri(endpoint)
                If id IsNot Nothing Then
                    uri += "/" + id.ToString()
                End If

                uri += BuildQueryString(queryString)
                Dim response = Await client.GetAsync(uri)

                Return BuildResponse(Of T)(response)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of T)(ex)
        End Try
    End Function

    Public Async Function PostAsync(Of T, V)(data As T, Optional endpoint As String = "") As Task(Of ResultInfo(Of V, Status))
        Try
            Using client = GetHttpClient()
                Dim uri As String = ConstructUri(endpoint)
                Dim content As HttpContent = New StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await client.PostAsync(uri, content)
                Return BuildResponse(Of V)(response)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of V)(ex)
        End Try
    End Function

    Public Async Function PutAsync(Of T, V)(id As Object, data As T) As Task(Of ResultInfo(Of V, Status))
        Try
            Using client = GetHttpClient()
                Dim uri As String = ConstructUri()

                uri += "/" + id.ToString()
                Dim content As HttpContent = New StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await client.PutAsync(uri, content)
                Return BuildResponse(Of V)(response)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of V)(ex)
        End Try
    End Function

    Public Async Function PatchAsync(Of T, V)(data As T) As Task(Of ResultInfo(Of V, Status))
        Try
            Using client = GetHttpClient()
                Dim uri As String = ConstructUri()

                Dim request As HttpRequestMessage = New HttpRequestMessage(New HttpMethod("PATCH"), uri)
                request.Content = New StringContent(JsonConvert.SerializeObject(data))
                Dim response As HttpResponseMessage = Await client.SendAsync(request)
                Return BuildResponse(Of V)(response)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of V)(ex)
        End Try
    End Function

    Public Async Function DeleteAsync(id As Object) As Task(Of ResultInfo(Of Boolean, Status))
        Try
            Using client = GetHttpClient()
                Dim uri As String = ConstructUri()
                uri += "/" + id.ToString()
                Dim response As HttpResponseMessage = Await client.DeleteAsync(uri)
                Return BuildResponse(Of Boolean)(response)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

#End Region

    Public Function [Get](Of T)(Optional endpoint As String = "", Optional id As Object = Nothing, Optional queryString As IDictionary(Of String, Object) = Nothing) As ResultInfo(Of T, Status)
        Dim _task = Task.Run(Function()
                                 Return GetAsync(Of T)(endpoint, id, queryString)
                             End Function)

        _task.Wait()
        Dim result = _task.Result
        Return result
    End Function

    Public Function Post(Of T, V)(data As T, Optional endpoint As String = "") As ResultInfo(Of V, Status)
        Dim _task = Task.Run(Function()
                                 Return PostAsync(Of T, V)(data, endpoint)
                             End Function)

        _task.Wait()
        Dim result = _task.Result
        Return result
    End Function

    Public Function Put(Of T, V)(id As Object, data As T) As ResultInfo(Of V, Status)
        Dim _task = Task.Run(Function()
                                 Return PutAsync(Of T, V)(id, data)
                             End Function)

        _task.Wait()
        Dim result = _task.Result
        Return result
    End Function

    Public Function Patch(Of T, V)(data As T) As ResultInfo(Of V, Status)
        Dim _task = Task.Run(Function()
                                 Return PatchAsync(Of T, V)(data)
                             End Function)

        _task.Wait()
        Dim result = _task.Result
        Return result
    End Function

    Public Function Delete(id As Object) As ResultInfo(Of Boolean, Status)
        Dim _task = Task.Run(Function()
                                 Return DeleteAsync(id)
                             End Function)

        _task.Wait()
        Dim result = _task.Result
        Return result
    End Function

    Protected Function GetHttpClient() As HttpClient
        Dim client = New HttpClient()
        client.Timeout = timeout
        client.BaseAddress = New Uri(BaseAddress)

        BuildHeaders(client)

        client.DefaultRequestHeaders.Accept.Clear()
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

        Return client
    End Function

    Protected Overridable Sub BuildHeaders(client As HttpClient)
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken)

        'client.DefaultRequestHeaders.Add("AccountName", "autodeskdemo1")

        If Not String.IsNullOrWhiteSpace(Me.UserEmail) Then
            client.DefaultRequestHeaders.Add("useremailid", Me.UserEmail)
        End If

    End Sub


    Protected Function ConstructUri(Optional endpoint As String = "")
        Dim path = String.Format("{0}/{1}",baseAddress,Me.path)
        If path Is Nothing Then
            path = ""
        End If
        If String.IsNullOrWhiteSpace(endpoint) = False Then
            path += "/" + endpoint
        End If
        Return path
    End Function

    Private Function BuildQueryString(queryString As IDictionary(Of String, Object)) As String
        Dim r As String = ""

        If (queryString IsNot Nothing) Then
            Dim list = New List(Of String)()

            For Each key As String In queryString.Keys
                list.Add(key + "=" + queryString(key).ToString())
            Next

            r = "?" + String.Join("&", list)
        End If

        Return r
    End Function


    Protected Overridable Function BuildResponse(Of TT)(response As HttpResponseMessage) As ResultInfo(Of TT, Status)
        If response.StatusCode = Net.HttpStatusCode.OK Then
            'Dim result1 = response.Content.ReadAsAsync(Of TT).Result
            Dim result = JsonConvert.DeserializeObject(Of ResultInfo(Of TT, Status))(response.Content.ReadAsStringAsync().Result)
            Return result
        Else
            Dim result As New ResultInfo(Of TT, Status)()
            result.Status = Status.Error
            Return result
        End If
    End Function

    Protected Function BuildResponse(Of TT)(message As String) As ResultInfo(Of TT, Status)
        Dim resultInfo As New ResultInfo(Of TT, Status)
        resultInfo.Message = message
        Return resultInfo
    End Function

    Protected Function BuildExceptionResponse(Of TT)(ex As Exception) As ResultInfo(Of TT, Status)
        Dim e = ex
        While e.InnerException IsNot Nothing
            e = e.InnerException
        End While

        Return BuildResponse(Of TT)(e.Message)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub

End Class
