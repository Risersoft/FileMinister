Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums

''' <summary>
''' FileAgent Repository
''' </summary>
''' <remarks></remarks>
Public Class AgentRepository
    Inherits ServerRepositoryBase(Of FileAgentInfo, Guid)
    Implements IAgentRepository

    ''' <summary>
    ''' Get All FileAgent Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of FileAgentInfo), Status)

        Try
            Using service = GetServerEntity()
                Dim result = service.FileAgents.Where(Function(p) p.IsDeleted = False).Select(Function(s) New FileAgentInfo With {
                    .FileAgentId = s.FileAgentId,
                    .AgentName = s.AgentName
                 }).ToList()
                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of FileAgentInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get FileAgent details
    ''' </summary>
    ''' <param name="id">FileAgentId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Guid) As ResultInfo(Of FileAgentInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim FileAgent = service.FileAgents.FirstOrDefault(Function(p) p.FileAgentId = id)
                Dim obj = New FileAgentInfo
                If FileAgent IsNot Nothing Then
                    obj = MapToObject(FileAgent)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of FileAgentInfo)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Add FileAgent
    ''' </summary>
    ''' <param name="data">FileAgent Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As FileAgentInfo) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try

                Using service = GetServerEntity()

                    Dim isAgentExist = service.FileAgents.Any(Function(p) p.AgentName = data.AgentName AndAlso p.IsDeleted = False)

                    If (Not isAgentExist) Then
                        'for entry in FileAgent role assignment table
                        Dim FileAgent = MapFromObject(data)
                        FileAgent.IsDeleted = False
                        FileAgent.CreatedByUserId = Me.User.UserId
                        FileAgent.CreatedOnUTC = DateTime.UtcNow

                        For Each oAgentShareInfo As FileAgentShareInfo In data.AgentShares
                            Dim oAgentShare = New FileAgentShare()
                            oAgentShare.FileAgentId = FileAgent.FileAgentId
                            oAgentShare.FileShareId = oAgentShareInfo.FileShareId
                            oAgentShare.CreatedByUserId = Me.User.UserId
                            oAgentShare.CreatedOnUTC = DateTime.UtcNow
                            oAgentShare.IsDeleted = False
                            FileAgent.FileAgentShares.Add(oAgentShare)
                        Next

                        'for entry in mac addresses assignment table
                        If data.WorkSpaces.Count > 0 Then
                            For Each WorkSpace As WorkSpaceInfo In data.WorkSpaces
                                Dim WorkSpaceObj = New WorkSpace()
                                WorkSpaceObj.WorkSpaceId = Guid.NewGuid()
                                WorkSpaceObj.FileAgentId = FileAgent.FileAgentId
                                WorkSpaceObj.MacAddresses = WorkSpace.MacAddresses
                                WorkSpaceObj.CreatedByUserId = Me.User.UserId
                                WorkSpaceObj.CreatedOnUTC = WorkSpace.CreatedOnUTC
                                WorkSpaceObj.IsDeleted = False
                                FileAgent.WorkSpaces.Add(WorkSpaceObj)
                            Next
                        End If

                        service.FileAgents.Add(FileAgent)
                        service.SaveChanges()

                        Return BuildResponse(True)
                    Else
                        Return BuildResponse(False, Status.AlreadyExists, "FileAgent already exists")
                    End If
                End Using

            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If
    End Function

    ''' <summary>
    ''' Update FileAgent Details
    ''' </summary>
    ''' <param name="id">FileAgentId</param>
    ''' <param name="data">FileAgent Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update(id As Guid, data As FileAgentInfo) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try
                Using service = GetServerEntity()
                    Dim FileAgent = service.FileAgents.FirstOrDefault(Function(p) p.FileAgentId = id)
                    If FileAgent IsNot Nothing Then
                        If Not String.IsNullOrWhiteSpace(data.SecretKey) Then
                            FileAgent.SecretKey = data.SecretKey
                        End If

                        Dim macAddrArr = data.WorkSpaces.Where(Function(p) p.WorkSpaceId <> Guid.Empty).Select(Function(p) p.WorkSpaceId).ToList()

                        Dim existingData = service.WorkSpaces.Where(Function(p) macAddrArr.Contains(p.WorkSpaceId) And p.FileAgentId = data.FileAgentId And p.IsDeleted = False)
                        For Each ama In existingData
                            Dim d = data.WorkSpaces.FirstOrDefault(Function(p) p.WorkSpaceId = ama.WorkSpaceId)
                            ama.MacAddresses = d.MacAddresses
                        Next

                        Dim missingData = service.WorkSpaces.Where(Function(p) Not macAddrArr.Contains(p.WorkSpaceId) And p.FileAgentId = data.FileAgentId And p.IsDeleted = False)
                        For Each ama In missingData
                            ama.IsDeleted = True
                            ama.DeletedByUserId = Me.User.UserId
                            ama.DeletedOnUTC = DateTime.UtcNow
                        Next

                        For Each WorkSpace In data.WorkSpaces.Where(Function(p) p.WorkSpaceId = Guid.Empty)
                            Dim WorkSpaceObj = New WorkSpace()
                            WorkSpaceObj.WorkSpaceId = Guid.NewGuid()
                            WorkSpaceObj.FileAgentId = FileAgent.FileAgentId
                            WorkSpaceObj.MacAddresses = WorkSpace.MacAddresses
                            WorkSpaceObj.CreatedByUserId = Me.User.UserId
                            WorkSpaceObj.CreatedOnUTC = WorkSpace.CreatedOnUTC
                            WorkSpaceObj.IsDeleted = False
                            FileAgent.WorkSpaces.Add(WorkSpaceObj)
                        Next

                        Dim shareArr = data.AgentShares.Where(Function(p) p.FileAgentShareId <> Nothing).Select(Function(p) p.FileShareId).ToList()
                        Dim missingShareData = service.FileAgentShares.Where(Function(p) Not shareArr.Contains(p.FileShareId) And p.FileAgentId = data.FileAgentId And p.IsDeleted = False)
                        For Each ama In missingShareData
                            ama.IsDeleted = True
                            ama.DeletedByUserId = Me.User.UserId
                            ama.DeletedOnUTC = DateTime.UtcNow
                        Next

                        For Each newShare In data.AgentShares.Where(Function(p) p.FileAgentShareId = Nothing)
                            Dim oAgentShare = New FileAgentShare()
                            oAgentShare.FileAgentId = newShare.FileAgentId
                            oAgentShare.FileShareId = newShare.FileShareId
                            oAgentShare.CreatedByUserId = Me.User.UserId
                            oAgentShare.CreatedOnUTC = DateTime.UtcNow
                            oAgentShare.IsDeleted = False
                            FileAgent.FileAgentShares.Add(oAgentShare)
                        Next

                        service.SaveChanges()
                    End If
                    Return BuildResponse(True)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If
    End Function

    ''' <summary>
    ''' Delete FileAgent
    ''' </summary>
    ''' <param name="id">FileAgentId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Delete(id As Guid) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try
                Using service = GetServerEntity()
                    Dim FileAgent = service.FileAgents.FirstOrDefault(Function(p) p.FileAgentId = id)
                    If FileAgent IsNot Nothing Then
                        FileAgent.IsDeleted = True
                        FileAgent.DeletedByUserId = Me.User.UserId
                        FileAgent.DeletedOnUTC = DateTime.UtcNow()

                        For Each item In FileAgent.FileAgentShares
                            item.IsDeleted = True
                            item.DeletedByUserId = Me.User.UserId
                            item.DeletedOnUTC = DateTime.UtcNow()

                        Next

                        service.SaveChanges()
                    End If
                    Return BuildResponse(True)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If

    End Function

    ''' <summary>
    ''' Get all FileShares Mapped to FileAgent
    ''' </summary>
    ''' <param name="FileAgentId">FileAgentId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSharesByAgentId(FileAgentId As Guid) As ResultInfo(Of List(Of ConfigInfo), Status) Implements IAgentRepository.GetSharesByAgentId

        Try
            Using service = GetServerEntity()
                Dim q = service.FileShares.Include("AgentShares")

                Dim data As New List(Of ConfigInfo)
                For Each p In q
                    Dim FileShare As New ConfigInfo
                    With FileShare
                        .FileShareId = p.FileShareId
                        .ShareName = p.ShareName
                        .CreatedByUserId = p.CreatedByUserId
                        .CreatedOnUTC = p.CreatedOnUTC
                        .IsDeleted = p.IsDeleted
                        .DeletedByUserId = p.DeletedByUserId
                        .DeletedOnUTC = p.DeletedOnUTC
                        .AgentShares = p.FileAgentShares.Select(Function(a) New FileAgentShareInfo With {
                            .FileAgentShareId = a.FileAgentShareId,
                            .FileAgentId = a.FileAgentId,
                            .FileShareId = a.FileShareId,
                            .CreatedByUserId = a.CreatedByUserId,
                            .CreatedOnUTC = a.CreatedOnUTC,
                            .IsDeleted = a.IsDeleted,
                            .DeletedByUserId = a.DeletedByUserId,
                            .DeletedOnUTC = a.DeletedOnUTC
                        }).ToList()
                    End With
                    data.Add(FileShare)
                Next
                Return BuildResponse(data)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of ConfigInfo))(ex)
        End Try


    End Function

    ''' <summary>
    ''' Map FileAgent Details to FileAgent Model
    ''' </summary>
    ''' <param name="s">FileAgent Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As FileAgent) As FileAgentInfo

        Dim t = New FileAgentInfo With {
               .FileAgentId = s.FileAgentId,
               .AgentName = s.AgentName
           }
        If (s.WorkSpaces IsNot Nothing And s.WorkSpaces.Count > 0) Then
            For Each item In s.WorkSpaces
                If (item.IsDeleted = False) Then
                    Dim a As New WorkSpaceInfo()
                    a.WorkSpaceId = item.WorkSpaceId
                    a.MacAddresses = item.MacAddresses
                    t.WorkSpaces.Add(a)
                End If
            Next
        End If
        If (s.FileAgentShares IsNot Nothing And s.FileAgentShares.Count > 0) Then
            For Each item In s.FileAgentShares
                If (item.IsDeleted = False) Then
                    Dim a As FileAgentShareInfo = New FileAgentShareInfo()
                    a.FileAgentShareId = item.FileAgentShareId
                    a.FileShareId = item.FileShareId
                    a.ShareName = item.FileShare.ShareName
                    t.AgentShares.Add(a)
                End If
            Next
        End If
        Return t
    End Function

    ''' <summary>
    ''' Map FileAgent Model to FileAgent Entity
    ''' </summary>
    ''' <param name="s">FileAgent Model</param>
    ''' <param name="t">FileAgent Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromObject(s As FileAgentInfo, Optional t As FileAgent = Nothing) As FileAgent
        Dim tt = t
        If tt Is Nothing Then
            tt = New FileAgent()
        End If
        tt.FileAgentId = Guid.NewGuid()
        tt.AgentName = s.AgentName
        tt.CreatedOnUTC = DateTime.UtcNow
        tt.CreatedByUserId = Me.User.UserId
        tt.IsDeleted = s.IsDeleted
        tt.SecretKey = s.SecretKey

        Return tt
    End Function

    ''' <summary>
    ''' Validate FileAgent
    ''' </summary>
    ''' <param name="agentId">AgentID</param>
    ''' <param name="secretKey">Secret Key</param>
    ''' <param name="macAddresses">Mac Address List</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateAgentWithMAC(agentId As Guid, secretKey As String, macAddresses As List(Of String)) As ResultInfo(Of WorkSpaceInfo, Status) Implements IAgentRepository.ValidateAgentWithMAC
        Try
            Using service = GetServerEntity()
                Dim WorkSpace As WorkSpaceInfo = Nothing
                Dim d = service.WorkSpaces.FirstOrDefault(Function(p) macAddresses.Contains(p.MacAddresses) And p.FileAgent.SecretKey = secretKey And p.FileAgent.FileAgentId = agentId And p.IsDeleted = False And p.FileAgent.IsDeleted = False)
                If d IsNot Nothing Then
                    WorkSpace = New WorkSpaceInfo() With {
                        .SelectedAgentId = d.FileAgentId,
                        .SelectedAgentName = d.FileAgent.AgentName,
                        .MacAddresses = d.MacAddresses,
                        .WorkSpaceId = d.WorkSpaceId,
                        .ServerName = d.ServerName,
                        .UserDomainId = d.UserDomainId,
                        .UserDomain = d.UserDomain.DomainName
                    }

                End If
                Return BuildResponse(WorkSpace)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of WorkSpaceInfo)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Get All FileAgent FileShare Mapped Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [AgentMapDetails]() As ResultInfo(Of List(Of AgentShareMappingInfo), Status) Implements IAgentRepository.AgentMapDetails

        Try
            Using service = GetServerEntity()
                Dim agentShareAggregate = New List(Of AgentShareMappingInfo)

                Dim agentShares = New List(Of AgentShareMappingInfo)
                Dim data = service.FileAgents.Where(Function(p) p.IsDeleted = False).ToList()
                For Each dt In data
                    Dim cnt = dt.FileAgentShares.Where(Function(p) p.IsDeleted = False).Count()
                    agentShares.Add(New AgentShareMappingInfo With {.ShareCount = cnt, .FileAgentId = dt.FileAgentId, .AgentName = dt.AgentName})
                Next

                Return BuildResponse(agentShares)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of AgentShareMappingInfo))(ex)
        End Try
    End Function

    Public Function GetLocalShares(workSpaceId As Guid) As ResultInfo(Of List(Of LocalShareInfo), Status) Implements IAgentRepository.GetLocalShares
        Dim result As List(Of LocalShareInfo) = Nothing
        Try
            Using service = GetServerEntity()
                Dim localshares = (From ls In service.LocalShares
                                   Join fs In service.FileShares On ls.FileShareId Equals fs.FileShareId
                                   Where ls.WorkSpaceId = workSpaceId
                                   Select ls, fs.ShareName)

                If localshares IsNot Nothing Then
                    result = New List(Of LocalShareInfo)()

                    For Each obj In localshares
                        result.Add(New LocalShareInfo With {
                            .Password = obj.ls.Password,
                            .ShareId = obj.ls.FileShareId,
                            .ShareName = obj.ShareName,
                            .SharePathLocal = obj.ls.SharePathLocal,
                            .SharePathUNC = obj.ls.SharePathUNC,
                            .TenantId = obj.ls.TenantID,
                            .WindowsUser = obj.ls.WindowsUser
                        })
                    Next

                Else
                    Return BuildResponse(result, Status.NotFound)
                End If

                Return BuildResponse(result, Status.Success)

            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of LocalShareInfo))(ex)
        End Try
    End Function

End Class
