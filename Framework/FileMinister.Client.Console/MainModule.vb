
Module MainModule

    Sub Main()
        ServiceManager.Start()

        System.Console.WriteLine("Service started..press any key to stop the service")

        System.Console.ReadKey()

        ServiceManager.Stop()

        System.Console.WriteLine("Service stopped.")

    End Sub

End Module
