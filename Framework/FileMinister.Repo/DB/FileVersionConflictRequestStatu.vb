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

Partial Public Class FileVersionConflictRequestStatu
    Public Property FileVersionConflictRequestStatusId As Byte
    Public Property TenantID As System.Guid
    Public Property FileVersionConflictRequestStatusName As String

    Public Overridable Property FileVersionConflictRequests As ICollection(Of FileVersionConflictRequest) = New HashSet(Of FileVersionConflictRequest)

End Class
