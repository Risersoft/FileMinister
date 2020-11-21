Imports System.Data.Entity
Imports risersoft.shared.dal

Partial Public Class FileMinisterEntities
    Inherits DbContext

    Public ReadOnly TenantId As Guid
    Public Sub New(strConn As String, TenantId As Guid)
        MyBase.New(strConn)
        Me.TenantId = TenantId
        AddHandler Me.Database.Connection.StateChange, AddressOf Connection_StateChange

        Me.Database.Log = Sub(log) System.Diagnostics.Debug.WriteLine(log)
    End Sub
    Sub Connection_StateChange(sender As Object, e As System.Data.StateChangeEventArgs)
        If e.CurrentState = System.Data.ConnectionState.Open Then
            TryCast(sender, System.Data.SqlClient.SqlConnection).SetSessionContext("TenantId", TenantId)
        End If
    End Sub

    Public Overrides Function SaveChanges() As Integer

        ChangeTracker.UpdateRLSID("TenantID", TenantId)

        For Each objEntry In ChangeTracker.Entries(Of FileAgent)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.FileAgentId = Guid.Empty Then
                objEntry.Entity.FileAgentId = Guid.NewGuid
            End If
        Next
        For Each objEntry In ChangeTracker.Entries(Of FileEntry)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.FileEntryId = Guid.Empty Then
                objEntry.Entity.FileEntryId = Guid.NewGuid
            End If
        Next
        For Each objEntry In ChangeTracker.Entries(Of FileVersion)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.FileVersionId = Guid.Empty Then
                objEntry.Entity.FileVersionId = Guid.NewGuid
            End If
        Next
        For Each objEntry In ChangeTracker.Entries(Of GroupFileEntryPermission)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.GroupFileEntryPermissionId = Guid.Empty Then
                objEntry.Entity.GroupFileEntryPermissionId = Guid.NewGuid
            End If
        Next
        For Each objEntry In ChangeTracker.Entries(Of UserFileEntryPermission)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.UserFileEntryPermissionId = Guid.Empty Then
                objEntry.Entity.UserFileEntryPermissionId = Guid.NewGuid
            End If
        Next
        For Each objEntry In ChangeTracker.Entries(Of Tag)
            If objEntry.State = Entity.EntityState.Added AndAlso objEntry.Entity.TagId = Guid.Empty Then
                objEntry.Entity.TagId = Guid.NewGuid
            End If
        Next

        Return MyBase.SaveChanges()
    End Function

End Class