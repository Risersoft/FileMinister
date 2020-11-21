Imports Newtonsoft.Json
Public Class DisplayData
    <JsonProperty("accountId")> _
    Public Property accountId() As Integer
        Get
            Return m_accountId
        End Get
        Protected Set(value As Integer)
            m_accountId = value
        End Set
    End Property
    Private m_accountId As Integer

    <JsonProperty("accountName")> _
    Public Property accountName() As String
        Get
            Return m_accountName
        End Get
        Protected Set(value As String)
            m_accountName = value
        End Set
    End Property
    Private m_accountName As String

    <JsonProperty("productId")> _
    Public Property productId() As Integer
        Get
            Return m_productId
        End Get
        Protected Set(value As Integer)
            m_productId = value
        End Set
    End Property
    Private m_productId As Integer

    <JsonProperty("productName")> _
    Public Property productName() As String
        Get
            Return m_productName
        End Get
        Protected Set(value As String)
            m_productName = value
        End Set
    End Property
    Private m_productName As String


    <JsonProperty("currencyName")> _
    Public Property currencyName() As String
        Get
            Return m_currencyName
        End Get
        Protected Set(value As String)
            m_currencyName = value
        End Set
    End Property
    Private m_currencyName As String

    <JsonProperty("symbol")> _
    Public Property symbol() As String
        Get
            Return m_symbol
        End Get
        Protected Set(value As String)
            m_symbol = value
        End Set
    End Property
    Private m_symbol As String

    <JsonProperty("email")> _
    Public Property email() As String
        Get
            Return m_email
        End Get
        Protected Set(value As String)
            m_email = value
        End Set
    End Property
    Private m_email As String

    <JsonProperty("expireDate")> _
    Public Property expireDate() As String
        Get
            Return m_expireDate
        End Get
        Protected Set(value As String)
            m_expireDate = value
        End Set
    End Property
    Private m_expireDate As String

    <JsonProperty("gracePeriod")> _
    Public Property gracePeriod() As String
        Get
            Return m_gracePeriod
        End Get
        Protected Set(value As String)
            m_gracePeriod = value
        End Set
    End Property
    Private m_gracePeriod As String

    <JsonProperty("licenseType")> _
    Public Property licenseType() As String
        Get
            Return m_licenseType
        End Get
        Protected Set(value As String)
            m_licenseType = value
        End Set
    End Property
    Private m_licenseType As String

    <JsonProperty("groupName")> _
    Public Property groupName() As String
        Get
            Return m_groupName
        End Get
        Protected Set(value As String)
            m_groupName = value
        End Set
    End Property
    Private m_groupName As String

    <JsonProperty("price")> _
    Public Property price() As Decimal
        Get
            Return m_price
        End Get
        Protected Set(value As Decimal)
            m_price = value
        End Set
    End Property
    Private m_price As Decimal

    <JsonProperty("purchaseDate")> _
    Public Property purchaseDate() As String
        Get
            Return m_purchaseDate
        End Get
        Protected Set(value As String)
            m_purchaseDate = value
        End Set
    End Property
    Private m_purchaseDate As String

    <JsonProperty("purchasedLicense")> _
    Public Property purchasedLicense() As String
        Get
            Return m_purchasedLicense
        End Get
        Protected Set(value As String)
            m_purchasedLicense = value
        End Set
    End Property
    Private m_purchasedLicense As String

    <JsonProperty("saConnectionstring")> _
    Public Property saConnectionstring() As String
        Get
            Return m_saConnectionstring
        End Get
        Protected Set(value As String)
            m_saConnectionstring = value
        End Set
    End Property
    Private m_saConnectionstring As String

    <JsonProperty("servicerURL")> _
    Public Property servicerURL() As String
        Get
            Return m_servicerURL
        End Get
        Protected Set(value As String)
            m_servicerURL = value
        End Set
    End Property
    Private m_servicerURL As String

    <JsonProperty("tierId")> _
    Public Property tierId() As Integer
        Get
            Return m_tierId
        End Get
        Protected Set(value As Integer)
            m_tierId = value
        End Set
    End Property
    Private m_tierId As Integer

    <JsonProperty("tierName")> _
    Public Property tierName() As String
        Get
            Return m_tierName
        End Get
        Protected Set(value As String)
            m_tierName = value
        End Set
    End Property
    Private m_tierName As String

    <JsonProperty("userId")> _
    Public Property userId() As Integer
        Get
            Return m_userId
        End Get
        Protected Set(value As Integer)
            m_userId = value
        End Set
    End Property
    Private m_userId As Integer

    <JsonProperty("paymentNotification")> _
    Public Property paymentNotification() As Boolean
        Get
            Return m_paymentNotification
        End Get
        Protected Set(value As Boolean)
            m_paymentNotification = value
        End Set
    End Property
    Private m_paymentNotification As Boolean

    <JsonProperty("createdDate")> _
    Public Property createdDate() As String
        Get
            Return m_createdDate
        End Get
        Protected Set(value As String)
            m_createdDate = value
        End Set
    End Property
    Private m_createdDate As String

End Class

Public Class ClientDbTest

    Public Property TableName() As String
        Get
            Return m_TableName
        End Get
        Set(value As String)
            m_TableName = value
        End Set
    End Property
    Private m_TableName As String

    Public Property ConnectionString() As String
        Get
            Return m_ConnectionString
        End Get
        Set(value As String)
            m_ConnectionString = value
        End Set
    End Property
    Private m_ConnectionString As String


End Class


Public Class Test
    <JsonProperty("Id")> _
    Public Property Id() As Integer
        Get
            Return m_Id
        End Get
        Set(value As Integer)
            m_Id = value
        End Set
    End Property
    Private m_Id As Integer
    <JsonProperty("Name")> _
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
End Class



Public Class TestStatus
    <JsonProperty("LstTest")> _
    Public Property LstTest() As List(Of Test)
        Get
            Return m_LstTest
        End Get
        Set(value As List(Of Test))
            m_LstTest = value
        End Set
    End Property
    Private m_LstTest As List(Of Test)
    <JsonProperty("ErrorMessage")> _
    Public Property ErrorMessage() As String
        Get
            Return m_ErrorMessage
        End Get
        Set(value As String)
            m_ErrorMessage = value
        End Set
    End Property
    Private m_ErrorMessage As String
End Class

Public Class ERPClientUser
    <JsonProperty("productName")> _
    Public Property productName() As String
        Get
            Return m_productName
        End Get
        Protected Set(value As String)
            m_productName = value
        End Set
    End Property
    Private m_productName As String
    <JsonProperty("productModuleName")> _
    Public Property productModuleName() As String
        Get
            Return m_productModuleName
        End Get
        Protected Set(value As String)
            m_productModuleName = value
        End Set
    End Property
    Private m_productModuleName As String
    <JsonProperty("groupName")> _
    Public Property groupName() As String
        Get
            Return m_groupName
        End Get
        Protected Set(value As String)
            m_groupName = value
        End Set
    End Property
    Private m_groupName As String

    <JsonProperty("servicerURL")> _
    Public Property servicerURL() As String
        Get
            Return m_servicerURL
        End Get
        Protected Set(value As String)
            m_servicerURL = value
        End Set
    End Property
    Private m_servicerURL As String

    <JsonProperty("serverName")> _
    Public Property serverName() As String
        Get
            Return m_serverName
        End Get
        Protected Set(value As String)
            m_serverName = value
        End Set
    End Property
    Private m_serverName As String
    <JsonProperty("databaseName")> _
    Public Property databaseName() As String
        Get
            Return m_databaseName
        End Get
        Protected Set(value As String)
            m_databaseName = value
        End Set
    End Property
    Private m_databaseName As String

    <JsonProperty("dbUser")> _
    Public Property dbUser() As String
        Get
            Return m_dbUser
        End Get
        Protected Set(value As String)
            m_dbUser = value
        End Set
    End Property
    Private m_dbUser As String

    <JsonProperty("dbPassword")> _
    Public Property dbPassword() As String
        Get
            Return m_dbPassword
        End Get
        Protected Set(value As String)
            m_dbPassword = value
        End Set
    End Property
    Private m_dbPassword As String
    <JsonProperty("connectionString")> _
    Public Property connectionString() As String
        Get
            Return m_connectionString
        End Get
        Protected Set(value As String)
            m_connectionString = value
        End Set
    End Property
    Private m_connectionString As String


End Class