Imports System.Timers
Imports Unity
Imports risersoft.shared.cloud.Cache
Imports risersoft.shared.agent

Namespace Util
    Public Class ServerDataSyncManager
        Dim timer As System.Timers.Timer
        Public Shared ReadOnly Instance As New ServerDataSyncManager()

        Private Sub New()
            timer = New System.Timers.Timer(30 * 1 * 60 * 1000)
            AddHandler timer.Elapsed, AddressOf Elapsed
        End Sub

        Public Sub ForceRun()
            If timer.Enabled Then
                Timeout()
            End If
        End Sub

        Public Sub Elapsed(sender As Object, e As ElapsedEventArgs)
            Timeout()
        End Sub

        Private Sub Timeout()
            Try
                timer.Enabled = False

                Dim cacheProvider = CallerCacheProvider.CreateInstance()

                Parallel.ForEach(cacheProvider.Values(), Sub(account)
                                                             SyncAccountData(account)
                                                         End Sub
                    )

            Catch ex As Exception

            Finally
                timer.Enabled = True
            End Try
        End Sub

        Public Sub SyncAccountData(user As RSCallerInfo)
            Dim taskUsers = ServiceAuthSyncProvider.AccountUserList(user)
            Dim taskGroups = ServiceAuthSyncProvider.AccountGroupList(user)


            Task.WaitAll(taskUsers, taskGroups)
            Dim t = Task.Run(Sub()
                                 Dim users = taskUsers.Result
                                 Dim groups = taskGroups.Result
                                 Dim repository = Helper.UnityContainer.Resolve(Of ISyncRepository)()
                                 repository.fncUser = Function() user
                                 repository.SyncServerData(users, groups)
                             End Sub)

            t.Wait()


        End Sub

        Public Sub Start()
            timer.Start()
        End Sub


        Public Sub [Stop]()
            timer.Stop()
        End Sub

    End Class
End Namespace