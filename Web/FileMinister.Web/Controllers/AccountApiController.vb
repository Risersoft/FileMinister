Imports System.Web.Http
Imports FileMinister.Models.Sync
Imports risersoft.shared.portable.Model

''' <summary>
''' Account Controller
''' </summary>
''' <remarks></remarks>
<RoutePrefix("api/Account")>
Public Class AccountController
    Inherits ServerApiController(Of BaseInfo, Integer, IAccountRepository)

    Public Sub New(repository As IAccountRepository)
        MyBase.New(repository)
    End Sub


    ''' <summary>
    ''' Get Account Storage used
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("blob-size")>
    <HttpGet>
    Public Function GetBlobSize() As IHttpActionResult
        Dim result = repository.GetBlobSize()
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get Account Quota limit
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GetAccountQuotaLimit")>
    <HttpGet>
    Public Function GetAccountQuotaLimit() As IHttpActionResult
        Dim result = Util.Helper.GetAccountQuotaLimit(Me.repository.User)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Get Application Settings
    ''' </summary>
    ''' <param name="key">Application Key Value</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("GetApplicationSettingValue")>
    <HttpGet>
    Public Function GetApplicationSettingValue(key As String) As IHttpActionResult
        Dim result = repository.GetApplicationSettingValue(key)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Set Application Settings
    ''' </summary>
    ''' <param name="data">Application Settings</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("SetApplicationSettingValue")>
    <HttpPost>
    Public Function SetApplicationSettingValue(data As Dictionary(Of String, String)) As IHttpActionResult
        Dim id = CType(data("Id"), Integer)
        Dim key = data("key").ToString()
        Dim value = data("value").ToString()
        Dim result = repository.SetApplicationSettingValue(id, key, value)
        Return Ok(result)
    End Function

    ''' <summary>
    ''' Set Account Quota Limit
    ''' </summary>
    ''' <param name="data">Account Quota Size</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Route("SetAccountQuotaLimit")>
    <HttpPost>
    Public Function SetAccountQuotaLimit(data As Long) As IHttpActionResult
        Dim result = Util.Helper.SetAccountQuotaLimit(data, Me.repository.User)
        Return Ok(result)
    End Function
End Class
