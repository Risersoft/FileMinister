Imports log4net
Imports log4net.Appender
Imports log4net.Config
Imports log4net.Core
Imports log4net.Filter
Imports log4net.Layout
Imports log4net.Repository
Imports log4net.Repository.Hierarchy
Imports risersoft.shared
''' <summary>
''' A static class that emulates defualt log4net LogManager static class.
''' The difference is that you can get various loggers istances based from an args.
''' LogMaster will create a different logger repository for each new arg it will receive.
''' </summary>
Public NotInheritable Class LogMaster
    Private Sub New()
    End Sub
    Public Shared Sub Configure(Name As String)
        LogMaster.Configure(Name, "ALL", Name)
    End Sub
    Public Shared Sub Configure(Name As String, levelName As String, fileName As String)
        Dim log As ILog = LogManager.GetLogger(Name & "Logger")
        Dim l As Logger = DirectCast(log.Logger, Logger)
        l.Level = l.Hierarchy.LevelMap(levelName)
        Dim appender As IAppender = LogMaster.CreateFileAppender(Name & "Appender", fileName)
        l.AddAppender(appender)
        l.Repository.Configured = True
        log.Info("Logger Created")
    End Sub

    ' Create a new file appender
    Public Shared Function CreateFileAppender(name As String, fileName As String) As IAppender

        Dim patterLayout As New log4net.Layout.PatternLayout()
        patterLayout.ConversionPattern = "%-5p %d %5rms %c{1} %M -%n%m%n%exception%n"
        patterLayout.ActivateOptions()

        Dim appender As New log4net.Appender.RollingFileAppender()
        appender.Layout = patterLayout
        appender.Name = name
        Dim commonPath = web.Globals.myApp.objAppExtender.CommonAppDataPath
        appender.File = IO.Path.Combine(commonPath, "Logs", fileName)
        appender.Encoding = Text.Encoding.UTF8
        appender.DatePattern = "_yyyy-MM-dd'.log'"
        appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date
        appender.StaticLogFileName = False
        appender.AppendToFile = True
        appender.MaximumFileSize = "10MB"
        appender.LockingModel = New RollingFileAppender.MinimalLock

        appender.ActivateOptions()

        Return appender
    End Function




    Private Shared Function GetMemoryAppender(station As String) As IAppender
        'MemoryAppender
        Dim memoryAppenderLayout = New PatternLayout("%date{HH:MM:ss} | %message%newline")
        memoryAppenderLayout.ActivateOptions()

        Dim memoryAppenderWithEventsName = String.Format("{0}{1}", "Memory", station)
        Dim levelRangeFilter = New LevelRangeFilter()
        levelRangeFilter.LevelMax = Level.Fatal
        levelRangeFilter.LevelMin = Level.Info

        Dim memoryAppenderWithEvents = New MemoryAppender
        memoryAppenderWithEvents.Name = memoryAppenderWithEventsName
        memoryAppenderWithEvents.AddFilter(levelRangeFilter)
        memoryAppenderWithEvents.Layout = memoryAppenderLayout
        memoryAppenderWithEvents.ActivateOptions()

        Return memoryAppenderWithEvents
    End Function
End Class
'http://stackoverflow.com/questions/308436/log4net-programmatically-specify-multiple-loggers-with-multiple-file-appenders