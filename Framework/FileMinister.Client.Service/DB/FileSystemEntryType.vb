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

Partial Public Class FileSystemEntryType
    Public Property FileSystemEntryTypeId As Byte
    Public Property FileSystemEntryTypeName As String
    Public Property IsFileSystemEntryContainerType As Boolean

    Public Overridable Property FileSystemEntries As ICollection(Of FileSystemEntry) = New HashSet(Of FileSystemEntry)
    Public Overridable Property Permissions As ICollection(Of Permission) = New HashSet(Of Permission)

End Class
