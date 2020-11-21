Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

''' <summary>
''' Account Repository
''' </summary>
''' <remarks></remarks>
Public Class AccountRepository
    Inherits ServerRepositoryBase(Of BaseInfo, Integer)
    Implements IAccountRepository

    ''' <summary>
    ''' Get Account Storage used
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBlobSize() As ResultInfo(Of ULong, Status) Implements IAccountRepository.GetBlobSize
        Return GetUsedQuota()
    End Function

    ''' <summary>
    '''Get Application Settings
    ''' </summary>
    ''' <param name="key">Application Key Value</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetApplicationSettingValue(key As String) As ResultInfo(Of String, Status) Implements IAccountRepository.GetApplicationSettingValue
        Try
            Using Service = GetServerEntity()
                Dim appsetting = Service.ApplicationSettings.FirstOrDefault(Function(p) p.ApplicationSettingKey = key)
                If appsetting IsNot Nothing Then
                    Return BuildResponse(Of String)(appsetting.ApplicationSettingValue)
                Else
                    Return BuildResponse(Of String)(String.Empty, Status.NotFound, "ApplicationSetting Not Found")
                End If
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of String)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Set Application Settings
    ''' </summary>
    ''' <param name="id">Application Setting Id</param>
    ''' <param name="key">Application Setting Key</param>
    ''' <param name="value">Application Setting Value</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetApplicationSettingValue(id As Integer, key As String, value As String) As ResultInfo(Of Boolean, Status) Implements IAccountRepository.SetApplicationSettingValue
        Try
            Using Service = GetServerEntity()
                Dim appsetting = Service.ApplicationSettings.FirstOrDefault(Function(p) p.ApplicationSettingKey = key)

                If appsetting IsNot Nothing Then
                    appsetting.ApplicationSettingValue = value
                    appsetting.CreatedOnUTC = Date.UtcNow()
                Else
                    Service.ApplicationSettings.Add(New ApplicationSetting() With {.ApplicationSettingId = id, .ApplicationSettingKey = key, .ApplicationSettingDesc = key, .ApplicationSettingValue = value, .CreatedOnUTC = Date.UtcNow()})
                End If

                Service.SaveChanges()

                Return BuildResponse(Of Boolean)(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try
    End Function

    Public Function GetUsedQuota() As ResultInfo(Of ULong, Status) Implements IAccountRepository.GetUsedQuota
        Try
            Dim result As ResultInfo(Of ULong, Status) = New ResultInfo(Of ULong, Status) With {.Data = 0, .Status = Status.Success}
            Using Service = GetServerEntity()
                result.Data = Service.FileShares.Where(Function(p) p.DiskSpaceMB IsNot Nothing).Sum(Function(p) p.DiskSpaceMB)
                Return result
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of ULong)(ex)
        End Try
    End Function

End Class
