Imports Newtonsoft.Json

Public Class ActiveUser
    <JsonProperty("email")> _
    Public Property Email() As String
        Get
            Return m_Email
        End Get
        Protected Set(value As String)
            m_Email = value
        End Set
    End Property
    Private m_Email As String

    <JsonProperty("userId")> _
    Public Property UserId() As Integer

        Get
            Return m_Id
        End Get
        Protected Set(value As Integer)
            m_Id = value
        End Set
    End Property
    Private m_Id As Integer

    <JsonProperty("accountId")> _
    Public Property AccountId() As Integer
        Get
            Return m_accountId
        End Get
        Protected Set(value As Integer)
            m_accountId = value
        End Set
    End Property
    Private m_accountId As Integer


    <JsonProperty("accountName")> _
    Public Property AccountName() As String
        Get
            Return m_accountName
        End Get
        Protected Set(value As String)
            m_accountName = value
        End Set
    End Property
    Private m_accountName As String

    <JsonProperty("type")> _
    Public Property Type() As String
        Get
            Return m_type
        End Get
        Protected Set(value As String)
            m_type = value
        End Set
    End Property
    Private m_type As String

    <JsonProperty("userTypeId")> _
    Public Property userTypeId() As Integer
        Get
            Return m_userTypeId
        End Get
        Protected Set(value As Integer)
            m_userTypeId = value
        End Set
    End Property
    Private m_userTypeId As Integer

    Public Property OrganisationServiceURL As String
    Public Property ServiceUrl As String
    Public Property SyncServiceUrl As String

End Class
Public Class WActiveUser
    Public Property ActiveList() As List(Of ActiveUser)
        Get
            Return m_ActiveList
        End Get
        Protected Set(value As List(Of ActiveUser))
            m_ActiveList = value
        End Set
    End Property
    Private m_ActiveList As List(Of ActiveUser)
End Class


Public Class UserProvider
    <JsonProperty("userId")> _
    Public Property UserId() As Integer
        Get
            Return m_userId
        End Get
        Protected Set(value As Integer)
            m_userId = value
        End Set
    End Property
    Private m_UserId As Integer


    <JsonProperty("username")> _
    Public Property UserName() As String
        Get
            Return m_UserName
        End Get
        Protected Set(value As String)
            m_UserName = value
        End Set
    End Property
    Private m_UserName As String

    <JsonProperty("isTwofactorAuthentication")> _
    Public Property IsTwofactorAuthentication() As Boolean
        Get
            Return m_IsTwofactorAuthentication
        End Get
        Protected Set(value As Boolean)
            m_IsTwofactorAuthentication = value
        End Set
    End Property
    Private m_IsTwofactorAuthentication As Boolean
    'otp
    <JsonProperty("otp")> _
    Public Property Otp() As String
        Get
            Return m_Otp
        End Get
        Protected Set(value As String)
            m_Otp = value
        End Set
    End Property
    Private m_Otp As String

    <JsonProperty("authenticationProviderId")> _
    Public Property AuthenticationProviderId() As String
        Get
            Return m_AuthenticationProviderId
        End Get
        Protected Set(value As String)
            m_AuthenticationProviderId = value
        End Set
    End Property
    Private m_AuthenticationProviderId As String

    <JsonProperty("password")> _
    Public Property Password() As String
        Get
            Return m_Password
        End Get
        Protected Set(value As String)
            m_Password = value
        End Set
    End Property
    Private m_Password As String



    <JsonProperty("accountId")> _
    Public Property AccountId() As Integer
        Get
            Return m_AccountId
        End Get
        Protected Set(value As Integer)
            m_AccountId = value
        End Set
    End Property
    Private m_AccountId As Integer

    <JsonProperty("emailID")> _
    Public Property EmailID() As String
        Get
            Return m_EmailID
        End Get
        Protected Set(value As String)
            m_EmailID = value
        End Set
    End Property
    Private m_EmailID As String
    'UserTypeId
    <JsonProperty("UserTypeId")> _
    Public Property UserTypeId() As String
        Get
            Return m_UserTypeId
        End Get
        Protected Set(value As String)
            m_UserTypeId = value
        End Set
    End Property
    Private m_UserTypeId As String
    'providerName

    <JsonProperty("providerName")> _
    Public Property ProviderName() As String
        Get
            Return m_ProviderName
        End Get
        Protected Set(value As String)
            m_ProviderName = value
        End Set
    End Property
    Private m_ProviderName As String
    <JsonProperty("providerKey")> _
    Public Property ProviderKey() As String
        Get
            Return m_ProviderKey
        End Get
        Protected Set(value As String)
            m_ProviderKey = value
        End Set
    End Property
    Private m_ProviderKey As String
    <JsonProperty("errormessage")> _
    Public Property ErrorMessage() As String
        Get
            Return m_ErrorMessage
        End Get
        Protected Set(value As String)
            m_ErrorMessage = value
        End Set
    End Property
    Private m_ErrorMessage As String
End Class
