Imports CloudSync.Common

Namespace Model
    Public Class FileInfo
        Inherits BaseInfo

        Public Property FileId As System.Guid
        Public Property FileTypeId As Byte
        Public Property CurrentVersionNumber As Integer
        Public Property IsCheckedOut As Boolean
        Public Property CheckedOutOnUTC As Nullable(Of Date)
        Public Property CheckedOutByUserId As Nullable(Of System.Guid)
        Public Property IsDeleted As Boolean
        Public Property DeletedOnUTC As Nullable(Of Date)
        Public Property DeletedByUserId As Nullable(Of System.Guid)
        Public Property SharePath As String
    End Class
End Namespace

