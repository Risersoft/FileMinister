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

Partial Public Class UserFileEntryPermission
    Public Property UserFileEntryPermissionId As System.Guid
    Public Property TenantID As System.Guid
    Public Property UserId As System.Guid
    Public Property FileEntryId As System.Guid
    Public Property AllowedPermissions As Byte
    Public Property DeniedPermissions As Byte
    Public Property CreatedOnUTC As Date
    Public Property CreatedByUserId As Nullable(Of System.Guid)
    Public Property IsDeleted As Boolean
    Public Property DeletedOnUTC As Nullable(Of Date)
    Public Property DeletedByUserId As Nullable(Of System.Guid)

    Public Overridable Property FileEntry As FileEntry

End Class
