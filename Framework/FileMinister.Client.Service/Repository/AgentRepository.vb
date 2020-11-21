Imports System.Data.SqlClient
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports Microsoft.Practices.Unity
Imports risersoft.shared.portable.Enums

Public Class AgentRepository
    Inherits ClientRepositoryBase(Of FileAgentInfo, Guid)
    Implements IAgentRepository

    ''' <summary>
    ''' Synchrozing all shares against an agent
    ''' </summary>
    ''' <param name="agentId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function SyncShares(agentId As Guid) As ResultInfo(Of Boolean, Status) Implements IAgentRepository.SyncShares
        Dim userRepository = Helper.UnityContainer.Resolve(Of IUserRepository)()
        userRepository.User = Me.User
        Dim userResult = userRepository.GetUserByUserAccountId()
        If userResult.Status = Status.Success AndAlso userResult.Data IsNot Nothing Then
            Using serverAgent As New AgentClient(userResult.Data)
                Dim result = serverAgent.GetSharesByAgentId(agentId)
                If result.Status = 200 AndAlso result.Data IsNot Nothing Then
                    Return SaveShares(agentId, userResult.Data, result.Data)
                End If
                Return New ResultInfo(Of Boolean, Status)() With {.Status = result.Status, .Message = result.Message}
            End Using
        End If
        Return New ResultInfo(Of Boolean, Status)() With {.Status = userResult.Status, .Message = userResult.Message}
    End Function

    ''' <summary>
    ''' Saving all shares against an account and then make corresponding changes in UserAgent
    ''' </summary>
    ''' <param name="agentId"></param>
    ''' <param name="dbUser"></param>
    ''' <param name="shares"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SaveShares(agentId As Guid, dbUser As LocalWorkSpaceInfo, shares As List(Of ConfigInfo)) As ResultInfo(Of Boolean, Status)
        Try
            Using service = GetClientCommonEntity()

                Dim dtShares = New DataTable()
                dtShares.Columns.Add("ShareId", GetType(Short))
                dtShares.Columns.Add("ShareName", GetType(String))
                dtShares.Columns.Add("CreatedOnUTC", GetType(DateTime))
                dtShares.Columns.Add("CreatedByUserId", GetType(Integer))
                dtShares.Columns.Add("IsDeleted", GetType(Boolean))


                Dim dtAgentShares = New DataTable()
                dtAgentShares.Columns.Add("AgentShareId", GetType(Integer))
                dtAgentShares.Columns.Add("AgentId", GetType(Guid))
                dtAgentShares.Columns.Add("ShareId", GetType(Short))
                dtAgentShares.Columns.Add("CreatedOnUTC", GetType(DateTime))
                dtAgentShares.Columns.Add("CreatedByUserId", GetType(Integer))
                dtAgentShares.Columns.Add("IsDeleted", GetType(Boolean))

                For Each share In shares
                    Dim row = dtShares.NewRow()
                    row.BeginEdit()

                    row("ShareId") = share.FileShareId
                    row("ShareName") = share.ShareName
                    row("CreatedByUserId") = share.CreatedByUserId
                    row("CreatedOnUTC") = share.CreatedOnUTC
                    row("IsDeleted") = share.IsDeleted

                    row.EndEdit()

                    dtShares.Rows.Add(row)

                    For Each agentShare In share.AgentShares
                        Dim row2 = dtAgentShares.NewRow()

                        row2.BeginEdit()

                        row2("AgentShareId") = agentShare.FileAgentShareId
                        row2("AgentId") = agentShare.FileAgentId
                        row2("ShareId") = agentShare.FileShareId
                        row2("CreatedByUserId") = agentShare.CreatedByUserId
                        row2("CreatedOnUTC") = agentShare.CreatedOnUTC
                        row2("IsDeleted") = agentShare.IsDeleted

                        row2.EndEdit()
                        dtAgentShares.Rows.Add(row2)
                    Next

                Next

                Dim paramAccountId = New SqlParameter("@AccountId", SqlDbType.Int)
                paramAccountId.Value = dbUser.AccountId

                Dim paramShares = New SqlParameter("@Shares", SqlDbType.Structured)
                paramShares.Value = dtShares
                paramShares.TypeName = "dbo.Share"

                Dim paramAgentShares = New SqlParameter("@AgentShares", SqlDbType.Structured)
                paramAgentShares.Value = dtAgentShares
                paramAgentShares.TypeName = "dbo.AgentShare"

                Dim result = service.Database.ExecuteSqlCommand("exec dbo.usp_UPD_AgentShare @AccountId = @AccountId, @Shares = @Shares, @AgentShares = @AgentShares", paramAccountId, paramShares, paramAgentShares)

                Dim userAgents = service.UserAgents.Where(Function(p) p.UserAccountId = dbUser.UserAccountId)
                For Each ua In userAgents
                    SyncUserAgent(service, ua.AgentId, dbUser)
                Next
                If agentId <> Guid.Empty Then
                    SyncUserAgent(service, agentId, dbUser)
                End If

                service.SaveChanges()
                Return BuildResponse(True)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Add (New) and remove (Non Existing) from UserAgent
    ''' </summary>
    ''' <param name="service"></param>
    ''' <param name="agentId"></param>
    ''' <param name="dbUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SyncUserAgent(service As FileMinisterClientCommonEntities, agentId As Guid, dbUser As LocalWorkSpaceInfo)
        Dim userAccountId = dbUser.UserAccountId

        Dim agentShares = service.AgentShares.Where(Function(p) p.AgentId = agentId)
        Dim userAgents = service.UserAgents.Where(Function(p) p.UserAccountId = userAccountId)

        Dim q = From ash In agentShares
                Group Join uag In userAgents On ash.AgentId Equals uag.AgentId Into Group
                From ush In Group.DefaultIfEmpty()
                Where ush Is Nothing
                Select _agentId = ash.AgentId



        Dim agentName As String = Nothing
        If q.Count() > 0 Then
            Using agentClient As New AgentClient(dbUser)
                Dim agentResult = agentClient.Get(Of FileAgentInfo)(id:=agentId)
                If agentResult.Status = Status.Success Then
                    agentName = agentResult.Data.AgentName
                End If
            End Using
        End If
        For Each ua In q
            Dim userAgent = New UserAgent() With {
                                     .UserAgentId = Guid.NewGuid(),
                                     .AgentId = ua,
                                     .AgentName = agentName,
                                     .UserAccountId = userAccountId,
                                     .CreatedOnUTC = DateTime.UtcNow,
                                     .LastLoggedInUTC = DateTime.UtcNow
                                }

            service.UserAgents.Add(userAgent)
        Next

        Dim q2 = From uag In userAgents
                 Group Join ash In agentShares On uag.AgentId Equals ash.AgentId Into Group
                 From ash In Group.DefaultIfEmpty()
                 Where ash Is Nothing
                 Select uag

        service.UserAgents.RemoveRange(q2)

        Return True
    End Function

End Class
