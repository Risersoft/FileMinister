'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class GetLatestFileVersion_Result
    Public Property FileVersionId As System.Guid
    Public Property TenantID As System.Guid
    Public Property ParentFileEntryId As System.Guid
    Public Property FileEntryId As System.Guid
    Public Property VersionNumber As Integer
    Public Property PreviousFileVersionId As Nullable(Of System.Guid)
    Public Property FileEntrySize As Long
    Public Property FileEntryName As String
    Public Property FileEntryExtension As String
    Public Property FileEntryRelativePath As String
    Public Property FileEntryHash As String
    Public Property ServerFileName As Nullable(Of System.Guid)
    Public Property CreatedOnUTC As Date
    Public Property CreatedByUserId As System.Guid
    Public Property IsDeleted As Boolean
    Public Property DeletedOnUTC As Nullable(Of Date)
    Public Property DeletedByUserId As Nullable(Of System.Guid)
    Public Property FileEntryNameWithExtension As String

End Class
