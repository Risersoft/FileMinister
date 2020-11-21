Imports risersoft.app.mxent
Imports risersoft.shared
Imports risersoft.shared.cloud
Imports risersoft.shared.dal

Public Class clsAppExtenderFS
    Inherits clsAppExtendRsBaseETO
    Public Overrides Function StartupAppCode() As String
        Return ""
    End Function
    Public Overrides Function FileServerRequired() As Boolean
        Return False
    End Function
    Public Overrides Function AboutBox() As IForm
        Throw New NotImplementedException()
    End Function

    Public Overrides Function DataProvider(context As clsAppController, cb As IAsyncWCFCallBack) As IAppDataProvider
        Dim Provider As IAppDataProvider = New clsAppDataProvider2(context, cb)
        Return Provider

    End Function

    Public Overrides Function dicFormModelTypes() As clsCollecString(Of Type)
    End Function

    Public Overrides Function dicReportProviderTypes(myContext As clsAppController) As clsCollecString(Of Type)
    End Function

    Public Overrides Function dicTaskProviderTypes() As clsCollecString(Of Type)
    End Function


    Public Overrides Function NewDBName() As String
        Return "sedmsdb"
    End Function


    Public Overrides Function MinDBVersion() As Decimal
        Return My.Settings.MinDBVersion
    End Function

    Public Overrides Function LinkLabel() As String
        Throw New NotImplementedException()
    End Function

    Public Overrides Function ProgramCode() As String
        Return "fmxp"
    End Function

    Public Overrides Function ProgramName() As String
        Return "FileMinister"
    End Function
    Public Overrides Function TargetPlatform() As String
        Return "Windows"
    End Function
End Class
