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

Partial Public Class FileVersionConflictRequest
    Public Property FileVersionConflictId As System.Guid
    Public Property TenantID As System.Guid
    Public Property FileVersionConflictRequestStatusId As Byte
    Public Property FileEntrySize As Nullable(Of Long)
    Public Property FileEntryHash As String
    Public Property RequestedBy As System.Guid
    Public Property RequestedAtUTC As Date

    Public Overridable Property FileVersionConflict As FileVersionConflict
    Public Overridable Property FileVersionConflictRequestStatu As FileVersionConflictRequestStatu

End Class
