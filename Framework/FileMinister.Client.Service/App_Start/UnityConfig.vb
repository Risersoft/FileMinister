Imports System.Web.Http
Imports FileMinister.Client.Service.IRepository
Imports FileMinister.Client.Service.Repository
Imports Microsoft.Practices.Unity
Imports Unity.WebApi

Public Class UnityConfig
    Public Shared Sub RegisterComponents(config As HttpConfiguration)

        Dim container = New UnityContainer()
        Helper.UnityContainer = container
        ' register all your components with the container here
        ' it Is Not necessary to register your controllers

        ' e.g. container.RegisterType<ITestService, TestService>();
        'RegisterMockObjects(container)
        RegisterObjects(container)
        config.DependencyResolver = New UnityDependencyResolver(container)

    End Sub

    Private Shared Sub RegisterObjects(container As UnityContainer)
        container.RegisterType(Of IFileRepository, FileRepository)
        container.RegisterType(Of ITagRepository, TagRepository)()
        container.RegisterType(Of IConfigRepository, ConfigRepository)()
        container.RegisterType(Of IUserRepository, UserRepository)()
        container.RegisterType(Of IFileVersionRepository, FileVersionRepository)()
        container.RegisterType(Of IAgentRepository, AgentRepository)()
        container.RegisterType(Of ISyncRepository, SyncRepository)()
        container.RegisterType(Of IUserShareRepository, UserShareRepository)()
    End Sub

    Private Shared Sub RegisterMockObjects(container As UnityContainer)
        'container.RegisterType(Of IFileRepository, FileMockRepository)
    End Sub
End Class