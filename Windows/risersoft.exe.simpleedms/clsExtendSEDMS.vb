
Imports FileMinister.Client
Imports risersoft.app.mxform
Imports risersoft.app.mxform.eto
Imports risersoft.app.reports
Imports risersoft.app2.shared
Imports risersoft.shared.cloud
Imports risersoft.shared.dal
Imports risersoft.shared.portable.Proxies
Imports risersoft.shared.sync
Imports risersoft.shared.web
Imports risersoft.shared
Imports risersoft.app.shared.mxengg
Imports risersoft.app.mxent
Imports FileMinister.Repo
Imports System.Configuration

Public Class clsExtendSEDMS
    Inherits clsAppExtendRsBaseETO
    Protected Friend strApp As String = "", mFileProvider As ICloudFileProvider, mQueueProvider As IQueueProvider
    Dim dic As clsCollecString(Of Boolean)


    Public Overrides Function AboutBox() As risersoft.shared.IForm
        Return New frmAboutBox
    End Function


    Public Overrides Function ProgramName() As String
        Return "FileMinister"
    End Function


    Public Overrides Function ProgramCode() As String
        Return "mxfm"
    End Function

    Public Overrides Function LinkLabel() As String
        Return "http://www.fileminister.com"
    End Function
    Public Overrides Function OnAppInit(oApp As clsCoreApp) As Boolean
        Return myDWGView.RegisterCADImport()
    End Function
    Public Overrides Function WOTabList(oWO As clsWOInfoBase) As List(Of String)
        Dim tl As New List(Of String)
        tl.Add("edms")
        Return tl
    End Function
    Public Overrides Function FileProviderClient(context As IProviderContext, AppCode As String, ProviderCode As String) As clsFileProviderClientBase
        Dim provider As clsFileProviderClientBase
        Select Case ProviderCode.Trim.ToLower
            Case "blob"
                mFileProvider = ProviderFactory.CreateFileProvider(context)
                provider = New clsBlobFileClient(context, AppCode, mFileProvider)
            Case Else
                provider = MyBase.FileProviderClient(context, AppCode, ProviderCode)
        End Select
        Return provider
    End Function
    Public Overrides Function QueueProvider(context As IProviderContext) As IQueueProvider
        Dim setting1 As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("ServiceBus")
        If (mQueueProvider Is Nothing) AndAlso (setting1 IsNot Nothing) Then mQueueProvider = New clsAzureSBQProvider(context, setting1.ConnectionString)
        Return mQueueProvider
    End Function

    Public Overrides Function CreateDataProvider(context As clsAppController, cb As IAsyncWCFCallBack) As IAppDataProvider
        Dim Provider As IAppDataProvider = ProviderFactory.CreateDataProvider(context, cb)
        'AuthData.DataServiceUrl = context.App.dicUserSettings("cloudservice")
        'AuthData.Police = context.Police
        'AddHandler AuthData.Police.AccountSelected, AddressOf OnAccountSelected
        Return Provider

    End Function

    Private Sub OnAccountSelected(sender As Object, e As UserAccountProxy)
        Dim dic = win.Globals.myWinApp.Controller.DataProvider.DirectoryService
        'AuthData.SyncServiceUrl = "net.tcp://" & dic("SyncService")
        'AuthData.FileWebApiUrl = "http://" & dic("FileWebApi")
        'AuthData.AuthorityUrl = win.Globals.myWinApp.dicUserSettings("authority")
    End Sub

    Public Overrides Function dicFormModelTypes() As clsCollecString(Of Type)
        If dicFormModel Is Nothing Then
            dicFormModel = New clsCollecString(Of Type)
            Me.AddTypeAssembly(dicFormModel, GetType(frmFileShareModel).Assembly, GetType(clsFormDataModel))
        End If
        Return dicFormModel
    End Function
    Public Overrides Function dicReportProviderTypes(myContext As clsAppController) As clsCollecString(Of Type)
        If dicReportModelProvider Is Nothing Then
            dicReportModelProvider = New clsCollecString(Of Type)
            Me.AddTypeAssembly(dicReportModelProvider, GetType(pbomReportDataProvider).Assembly, GetType(clsReportDataProviderBase))
        End If
        Return dicReportModelProvider

    End Function

    Public Overrides Function FileServerRequired() As Boolean
        Return True
    End Function

    Public Overrides Function MinDBVersion() As Decimal

    End Function

    Public Overrides Function dicTaskProviderTypes() As clsCollecString(Of Type)

    End Function
End Class
