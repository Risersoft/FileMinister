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

Partial Public Class FileEntry
    Public Property FileEntryId As System.Guid
    Public Property TenantID As System.Guid
    Public Property FileEntryTypeId As Byte
    Public Property FileShareId As Integer
    Public Property CurrentVersionNumber As Integer
    Public Property IsCheckedOut As Boolean
    Public Property CheckedOutOnUTC As Nullable(Of Date)
    Public Property CheckedOutByUserId As Nullable(Of System.Guid)
    Public Property IsDeleted As Boolean
    Public Property DeletedOnUTC As Nullable(Of Date)
    Public Property DeletedByUserId As Nullable(Of System.Guid)
    Public Property IsPermanentlyDeleted As Boolean
    Public Property PermanentlyDeletedOnUTC As Nullable(Of Date)
    Public Property PermanentlyDeletedByUserId As Nullable(Of System.Guid)
    Public Property CheckedOutWorkSpaceId As Nullable(Of System.Guid)
    Public Property NumberHandles As Nullable(Of Byte)

    Public Overridable Property FileEntryType As FileEntryType
    Public Overridable Property FileShare As FileShare
    Public Overridable Property FileEntryLinks As ICollection(Of FileEntryLink) = New HashSet(Of FileEntryLink)
    Public Overridable Property FileEntryLinks1 As ICollection(Of FileEntryLink) = New HashSet(Of FileEntryLink)
    Public Overridable Property FileVersionConflicts As ICollection(Of FileVersionConflict) = New HashSet(Of FileVersionConflict)
    Public Overridable Property FileVersions As ICollection(Of FileVersion) = New HashSet(Of FileVersion)
    Public Overridable Property FileVersions1 As ICollection(Of FileVersion) = New HashSet(Of FileVersion)
    Public Overridable Property GroupFileEntryPermissions As ICollection(Of GroupFileEntryPermission) = New HashSet(Of GroupFileEntryPermission)
    Public Overridable Property Tags As ICollection(Of Tag) = New HashSet(Of Tag)
    Public Overridable Property UserFileEntryPermissions As ICollection(Of UserFileEntryPermission) = New HashSet(Of UserFileEntryPermission)
    Public Overridable Property FileConflicts As ICollection(Of FileConflict) = New HashSet(Of FileConflict)
    Public Overridable Property FileEntryDeleteGroupHierarchies As ICollection(Of FileEntryDeleteGroupHierarchy) = New HashSet(Of FileEntryDeleteGroupHierarchy)
    Public Overridable Property FileEntryDeleteGroupHierarchies1 As ICollection(Of FileEntryDeleteGroupHierarchy) = New HashSet(Of FileEntryDeleteGroupHierarchy)

End Class
