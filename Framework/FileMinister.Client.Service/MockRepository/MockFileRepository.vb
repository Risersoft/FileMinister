Namespace MockRepository
    Public Class FileMockRepository
        'Inherits MockRepositoryBase(Of FileSystemEntryInfo, Guid)
        'Implements IFileRepository

        'Public Function RemoveFileLink(fileSystemEntryId As Guid) As Boolean Implements IFileRepository.RemoveFileLink
        '    Return True
        'End Function

        'Public Overrides Function GetAll() As IList(Of FileSystemEntryInfo)
        '    Dim files = New List(Of FileSystemEntryInfo)()
        '    For i = 1 To 100
        '        files.Add(New FileSystemEntryInfo())
        '    Next
        '    Return files
        'End Function

        'Public Function GetByParentId(parentId As Guid) As List(Of FileSystemEntryInfo) Implements IFileRepository.GetByParentId
        '    Throw New NotImplementedException()
        'End Function

        'Public Function [FileSearch](search As FileSearch) As List(Of FileSystemEntryInfo) Implements IFileRepository.FileSearch
        '    Throw New NotImplementedException()
        'End Function
    End Class
End Namespace