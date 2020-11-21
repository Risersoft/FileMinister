Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable.Model
Public Class FileVersionClient
    Inherits ServiceClient
    Public Sub New()
        MyBase.New("api/FileVersion")
    End Sub

    Public Function [GetChekinsFileDetails](userId As Guid, shareId As Integer, fromDate As DateTime, toDate As DateTime) As ResultInfo(Of List(Of CheckinReportInfo), Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("userId", userId)
        obj.Add("shareId", shareId)
        obj.Add("fromDate", fromDate.ToString())
        obj.Add("toDate", toDate.ToString())

        Dim result = Me.Get(Of List(Of CheckinReportInfo))(endpoint:="GetChekinsFileDetails", queryString:=obj)
        Return result
    End Function

    Public Function [GetCheckoutFileDetails](userId As Guid, shareId As Integer) As ResultInfo(Of List(Of CheckinReportInfo), Status)
        Dim obj = New Dictionary(Of String, Object)
        obj.Add("userId", userId)
        obj.Add("shareId", shareId)

        Dim result = Me.Get(Of List(Of CheckinReportInfo))(endpoint:="GetCheckoutFileDetails", queryString:=obj)
        Return result
    End Function

End Class
