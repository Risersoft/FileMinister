Public Class Users
    Public Property Id() As Integer
        Get
            Return m_Id
        End Get
        Set(value As Integer)
            m_Id = value
        End Set
    End Property
    Private m_Id As Integer


    Public Property UserId() As Integer
        Get
            Return m_UserId
        End Get
        Set(value As Integer)
            m_UserId = value
        End Set
    End Property
    Private m_UserId As Integer
    'access_token

    Public Property access_token() As String
        Get
            Return m_access_token
        End Get
        Set(value As String)
            m_access_token = value
        End Set
    End Property
    Private m_access_token As String


    Public Property UserTypeId() As Integer
        Get
            Return m_UserTypeId
        End Get
        Set(value As Integer)
            m_UserTypeId = value
        End Set
    End Property
    Private m_UserTypeId As Integer

    Public Property AuthenticationProviderId() As Integer
        Get
            Return m_AuthenticationProviderId
        End Get
        Set(value As Integer)
            m_AuthenticationProviderId = value
        End Set
    End Property
    Private m_AuthenticationProviderId As Integer
    Public Property ProviderName() As String
        Get
            Return m_ProviderName
        End Get
        Set(value As String)
            m_ProviderName = value
        End Set
    End Property
    Private m_ProviderName As String
    Public Property Email() As String
        Get
            Return m_Email
        End Get
        Set(value As String)
            m_Email = value
        End Set
    End Property
    Private m_Email As String
    Public Property EmailConfirmed() As Boolean
        Get
            Return m_EmailConfirmed
        End Get
        Set(value As Boolean)
            m_EmailConfirmed = value
        End Set
    End Property
    Private m_EmailConfirmed As Boolean
    Public Property PasswordHash() As String
        Get
            Return m_PasswordHash
        End Get
        Set(value As String)
            m_PasswordHash = value
        End Set
    End Property
    Private m_PasswordHash As String


    Public Property Password() As String
        Get
            Return m_Password
        End Get
        Set(value As String)
            m_Password = value
        End Set
    End Property
    Private m_Password As String
    Public Property SecurityStamp() As String
        Get
            Return m_SecurityStamp
        End Get
        Set(value As String)
            m_SecurityStamp = value
        End Set
    End Property
    Private m_SecurityStamp As String
    Public Property PhoneNumber() As String
        Get
            Return m_PhoneNumber
        End Get
        Set(value As String)
            m_PhoneNumber = value
        End Set
    End Property
    Private m_PhoneNumber As String

    Public Property phone() As String
        Get
            Return m_phone
        End Get
        Set(value As String)
            m_phone = value
        End Set
    End Property
    Private m_phone As String

    Public Property PhoneNumberConfirmed() As Boolean
        Get
            Return m_PhoneNumberConfirmed
        End Get
        Set(value As Boolean)
            m_PhoneNumberConfirmed = value
        End Set
    End Property
    Private m_PhoneNumberConfirmed As Boolean
    Public Property AcessFailedCount() As Integer
        Get
            Return m_AcessFailedCount
        End Get
        Set(value As Integer)
            m_AcessFailedCount = value
        End Set
    End Property
    Private m_AcessFailedCount As Integer
    Public Property UserName() As String
        Get
            Return m_UserName
        End Get
        Set(value As String)
            m_UserName = value
        End Set
    End Property
    Private m_UserName As String
    Public Property IsTwoFactorAuthentication() As Boolean
        Get
            Return m_IsTwoFactorAuthentication
        End Get
        Set(value As Boolean)
            m_IsTwoFactorAuthentication = value
        End Set
    End Property
    Private m_IsTwoFactorAuthentication As Boolean
    Public Property IsDeleted() As Boolean
        Get
            Return m_IsDeleted
        End Get
        Set(value As Boolean)
            m_IsDeleted = value
        End Set
    End Property
    Private m_IsDeleted As Boolean
    Public Property CreatedBy() As String
        Get
            Return m_CreatedBy
        End Get
        Set(value As String)
            m_CreatedBy = value
        End Set
    End Property
    Private m_CreatedBy As String
    Public Property ModifiedBy() As String
        Get
            Return m_ModifiedBy
        End Get
        Set(value As String)
            m_ModifiedBy = value
        End Set
    End Property
    Private m_ModifiedBy As String
    Public Property AccountName() As String
        Get
            Return m_AccountName
        End Get
        Set(value As String)
            m_AccountName = value
        End Set
    End Property
    Private m_AccountName As String
    Public Property otp() As String
        Get
            Return m_otp
        End Get
        Set(value As String)
            m_otp = value
        End Set
    End Property
    Private m_otp As String
    Public Property generatedOtp() As String
        Get
            Return m_generatedOtp
        End Get
        Set(value As String)
            m_generatedOtp = value
        End Set
    End Property
    Private m_generatedOtp As String
    Public Property AccountId() As Integer
        Get
            Return m_AccountId
        End Get
        Set(value As Integer)
            m_AccountId = value
        End Set
    End Property
    Private m_AccountId As Integer
    Public Property AccessToken() As String
        Get
            Return m_AccessToken
        End Get
        Set(value As String)
            m_AccessToken = value
        End Set
    End Property
    Private m_AccessToken As String
    Public Property AccessTokenExpire() As String
        Get
            Return m_AccessTokenExpire
        End Get
        Set(value As String)
            m_AccessTokenExpire = value
        End Set
    End Property
    Private m_AccessTokenExpire As String
    Public Property ProviderKey() As String
        Get
            Return m_ProviderKey
        End Get
        Set(value As String)
            m_ProviderKey = value
        End Set
    End Property
    Private m_ProviderKey As String
    Public Property EmailID() As String
        Get
            Return m_EmailID
        End Get
        Set(value As String)
            m_EmailID = value
        End Set
    End Property
    Private m_EmailID As String
    Public Property ErrorMessage() As String
        Get
            Return m_ErrorMessage
        End Get
        Set(value As String)
            m_ErrorMessage = value
        End Set
    End Property
    Private m_ErrorMessage As String
    Public Property SessionId() As String
        Get
            Return m_SessionId
        End Get
        Set(value As String)
            m_SessionId = value
        End Set
    End Property
    Private m_SessionId As String

    Public Property AgentId As Guid
    Public Property AgentName As String
    Public Property MACAddress As String

End Class


Public Class UserProviderinfo
    Public Property id() As String
        Get
            Return m_id
        End Get
        Set(value As String)
            m_id = value
        End Set
    End Property
    Private m_id As String

End Class
