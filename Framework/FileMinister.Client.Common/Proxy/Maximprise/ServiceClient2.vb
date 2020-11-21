Imports System.Net.Http
Imports risersoft.shared.portable.Model
Imports Newtonsoft.Json
Imports risersoft.shared.portable.Enums
Imports FileMinister.Repo

Public MustInherit Class ServiceClient2
    Inherits WebApiClientResultServer

    Protected Friend m_User As LocalWorkSpaceInfo

    Public Overrides ReadOnly Property BaseAddress As String
        Get
            Dim url = GetServiceUrl()

            If url Is Nothing Then
                Throw New ArgumentNullException("Maximprise Service Url")
            End If

            Return url 'FileMinister.Common.Constants.AUTH_URI
        End Get
    End Property

    Public Sub New(path As String, Optional user As LocalWorkSpaceInfo = Nothing)
        MyBase.New(path)
        m_User = user
        Me.AccessToken = m_User.AccessToken
        Me.AccountName = m_User.AccountName
    End Sub

    Protected Overrides Function BuildResponse(Of TT)(response As HttpResponseMessage) As ResultInfo(Of TT, Status)
        Dim resultInfo As New ResultInfo(Of TT, Status)
        resultInfo.Status = response.StatusCode.ToString("D")
        If resultInfo.Status = Status.Success Then
            Dim result = JsonConvert.DeserializeObject(Of TT)(response.Content.ReadAsStringAsync().Result)
            resultInfo.Data = result
        End If

        Return resultInfo
    End Function

    Public MustOverride Function GetServiceUrl() As String

    'Public Shared Function GetServiceUrl()
    '    If serviceUrl Is Nothing Then
    '        serviceUrl = RegistryHelper.GetRegistryValue("SOFTWARE\FileMinister", "ServiceUrl")
    '    End If
    '    Return serviceUrl
    'End Function

End Class
