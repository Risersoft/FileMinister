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

Partial Public Class GetLatestFileSystemEntryVersionByPathAndClientPresenceFlag_Result
    Public Property FileSystemEntryId As System.Guid
    Public Property ShareId As Short
    Public Property FileSystemEntryTypeId As Byte
    Public Property CheckedOutByUserId As Nullable(Of System.Guid)
    Public Property CheckedOutOnUTC As Nullable(Of Date)
    Public Property IsCheckedOut As Boolean
    Public Property IsDeleted As Boolean
    Public Property IsPermanentlyDeleted As Boolean
    Public Property FileSystemEntryVersionId As System.Guid
    Public Property FileSystemEntryName As String
    Public Property FileSystemEntryExtension As String
    Public Property Name As String
    Public Property FileSystemEntryRelativePath As String
    Public Property ServerFileSystemEntryName As Nullable(Of System.Guid)
    Public Property FileSystemEntrySize As Long
    Public Property ParentFileSystemEntryId As System.Guid
    Public Property PreviousFileSystemEntryVersionId As Nullable(Of System.Guid)
    Public Property VersionNumber As Nullable(Of Integer)
    Public Property PrevVersionNumber As Nullable(Of Integer)
    Public Property FileSystemEntryHash As String
    Public Property CreatedOnUTC As Date
    Public Property CreatedByUserId As System.Guid
    Public Property FileSystemEntryStatusId As Nullable(Of Byte)
    Public Property IsConflicted As Boolean
    Public Property IsOpen As Boolean
    Public Property FileSystemEntryStatusDisplayName As String

End Class
