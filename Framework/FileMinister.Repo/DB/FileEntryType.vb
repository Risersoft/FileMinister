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

Partial Public Class FileEntryType
    Public Property FileEntryTypeId As Byte
    Public Property FileEntryTypeName As String
    Public Property IsFileEntryContainerType As Boolean

    Public Overridable Property FileEntries As ICollection(Of FileEntry) = New HashSet(Of FileEntry)
    Public Overridable Property Permissions As ICollection(Of Permission) = New HashSet(Of Permission)

End Class
