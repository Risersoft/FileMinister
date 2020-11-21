Imports risersoft.shared.portable.Model
Imports Microsoft.Reporting.WinForms
Imports System.Net.Mail
Imports FileMinister.Client.Common
Imports System.Reflection
Imports System.Configuration
Imports risersoft.shared.cloud
Imports risersoft.shared.portable.Proxies
Imports risersoft.shared.portable.Enums

Public Class ReportForm

    Dim ReportViewer1 = New ReportViewer()


    Private Sub ReportForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim accountName = AuthData.User.AccountName
        'LoadUsers(accountId)
        'LoadShares()



        Dim formatName As String = "pdf"

        For Each extension As RenderingExtension In ReportViewer1.LocalReport.ListRenderingExtensions

            If extension.Name.ToLower = formatName Then

                Dim m_isVisible As System.Reflection.FieldInfo = extension.GetType.GetField("m_isVisible", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)

                m_isVisible.SetValue(extension, False)

                Exit For

            End If

        Next


        Dim async = New AsyncProvider()
        async.AddMethod("Users", Function() New Common.OrganizationClient().GetAllAccountUser(accountName))
        async.AddMethod("Shares", Function() New Common.ShareClient().Get(Of List(Of ConfigInfo))())
        async.OnCompletion = Sub(list As IDictionary)
                                 Dim result = CType(list("Users"), ResultInfo(Of List(Of UserAccountProxy), Status))
                                 If ValidateResponse(result) Then
                                     Dim allUsers = New UserAccountProxy()
                                     allUsers.UserId = Guid.Empty
                                     allUsers.FullName = "All"
                                     result.Data.Add(allUsers)
                                     Users.DataSource = result.Data
                                     Users.DisplayMember = "FullName"
                                     Users.ValueMember = "UserId"
                                 End If

                                 Dim result1 = CType(list("Shares"), ResultInfo(Of List(Of ConfigInfo), Status))
                                 If ValidateResponse(result1) Then
                                     Dim allShares = New ConfigInfo()
                                     allShares.FileShareId = 0
                                     allShares.ShareName = "All"
                                     result1.Data.Add(allShares)
                                     Shares.DataSource = result1.Data
                                     Shares.DisplayMember = "ShareName"
                                     Shares.ValueMember = "ShareId"
                                 End If

                             End Sub
        async.Execute()

        Dim Reports = New Dictionary(Of String, Integer)
        Reports.Add("Checkins", 1)
        Reports.Add("Checkouts", 2)
        ReportTypes.DataSource = Reports.ToList()
        ReportTypes.DisplayMember = "Key"
        ReportTypes.ValueMember = "Value"


        FromTime.Format = DateTimePickerFormat.Custom
        FromTime.CustomFormat = "dd/MM/yyyy HH:mm"  '24 hour clock
        FromTime.Value = Date.Today.AddDays(-30)


        ToTime.Format = DateTimePickerFormat.Custom
        ToTime.CustomFormat = "dd/MM/yyyy HH:mm"
        Me.ReportViewer1.RefreshReport()
        PanelFilter.Controls.Remove(EmailReport_btn)
        PanelReport.Controls.Clear()

    End Sub

    Private Sub FilterBtn_Click(sender As Object, e As EventArgs) Handles Filterbtn.Click
        Dim selectedUser = Users.SelectedValue
        Dim selectedShare = Shares.SelectedValue
        Dim fromTime1 = FromTime.Value
        Dim toTime1 = ToTime.Value
        Dim selectedReport = ReportTypes.SelectedValue




        Using FileVersionProxy = New FileVersionClient()

            If ReportType.checkin = selectedReport Then
                Dim result = FileVersionProxy.GetChekinsFileDetails(selectedUser, selectedShare, fromTime1, toTime1).Data
                PanelReport.Controls.Clear()
                If result Is Nothing OrElse result.Count() = 0 Then
                    Dim label = New Label()
                    label.Text = "No Files Found"
                    PanelReport.Controls.Add(label)
                    PanelFilter.Controls.Remove(EmailReport_btn)
                Else

                    Dim assembly__1 = Assembly.GetExecutingAssembly()
                    Dim path = "FileMinister.Client.WinApp.CheckinReport.rdlc"
                    Dim stream = assembly__1.GetManifestResourceStream(path.ToString())
                    ReportViewer1.LocalReport.LoadReportDefinition(stream)



                    Dim datasource As New ReportDataSource("Checkins", result)
                    'Me.ReportViewer1.LocalReport.ReportPath = My.Resources.ReportPath + "\CheckinReport.rdlc" 'TODO report path change
                    Me.ReportViewer1.LocalReport.DataSources.Clear()
                    Me.ReportViewer1.LocalReport.DataSources.Add(datasource)
                    Me.ReportViewer1.Width = 800
                    'Me.ReportViewer1.Height = 300
                    Me.ReportViewer1.ZoomMode = ZoomMode.PageWidth


                    Me.ReportViewer1.RefreshReport()
                    PanelFilter.Controls.Add(EmailReport_btn)
                    PanelReport.Controls.Add(ReportViewer1)
                End If

            Else
                Dim result = FileVersionProxy.GetCheckoutFileDetails(selectedUser, selectedShare).Data
                PanelReport.Controls.Clear()
                If result Is Nothing Or result.Count() = 0 Then
                    Dim label = New Label()
                    label.Text = "No Files Found"
                    PanelReport.Controls.Add(label)
                    PanelFilter.Controls.Remove(EmailReport_btn)
                Else
                    Dim assembly__1 = Assembly.GetExecutingAssembly()
                    Dim path = "FileMinister.Client.WinApp.CheckoutReport.rdlc"
                    Dim stream = assembly__1.GetManifestResourceStream(path.ToString())
                    ReportViewer1.LocalReport.LoadReportDefinition(stream)

                    Dim datasource As New ReportDataSource("Checkout", result)
                    'Me.ReportViewer1.LocalReport.ReportPath = My.Resources.ReportPath + "\CheckoutReport.rdlc"
                    Me.ReportViewer1.LocalReport.DataSources.Clear()
                    Me.ReportViewer1.LocalReport.DataSources.Add(datasource)
                    Me.ReportViewer1.Width = 800
                    'Me.ReportViewer1.Height = 300
                    Me.ReportViewer1.ZoomMode = ZoomMode.PageWidth
                    Me.ReportViewer1.RefreshReport()
                    PanelFilter.Controls.Add(EmailReport_btn)
                    PanelReport.Controls.Add(ReportViewer1)
                End If

            End If

        End Using
    End Sub

    Private Sub LoadUsers(accountName As String)
        Dim async = New AsyncProvider()
        async.AddMethod("Users", Function() New Common.OrganizationClient().GetAllAccountUser(accountName))
        async.OnCompletion = Sub(list As IDictionary)
                                 Dim result = CType(list("Users"), ResultInfo(Of List(Of UserAccountProxy), Status))
                                 Shares.DataSource = result.Data
                                 Shares.DisplayMember = "FullName"
                                 Shares.ValueMember = "UserId"
                             End Sub
        async.Execute()
    End Sub

    Private Sub LoadShares()
        Dim async = New AsyncProvider()
        async.AddMethod("Shares", Function() New Common.ShareClient().Get(Of List(Of ConfigInfo))())
        async.OnCompletion = Sub(list As IDictionary)
                                 Dim result = CType(list("Shares"), ResultInfo(Of List(Of ConfigInfo), Status))
                                 If ValidateResponse(result) Then
                                     Shares.DataSource = result.Data
                                     Shares.DisplayMember = "ShareName"
                                     Shares.ValueMember = "ShareId"
                                 End If
                             End Sub
        async.Execute()
    End Sub

    Private Sub ReportTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ReportTypes.SelectedIndexChanged

        Dim selectedReportInedex = ReportTypes.SelectedIndex
        PanelFilter.Controls.Remove(EmailReport_btn)
        PanelReport.Controls.Clear()

        If ReportType.checkout = (selectedReportInedex + 1) Then
            FromTime.Enabled = False
            ToTime.Enabled = False
        Else
            FromTime.Enabled = True
            ToTime.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' Send Report in email
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SendPDFReport() As Boolean
        'Export to PDF/EXCEL. Get binary content.
        Dim excelContent As Byte() = ReportViewer1.LocalReport.Render("Excel", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
        Dim ms As New System.IO.MemoryStream(excelContent)
        Dim ct As New System.Net.Mime.ContentType("application/msexcel")
        Dim attach As New System.Net.Mail.Attachment(ms, ct)
        attach.ContentDisposition.FileName = "Report.xls"

        ''get loggedIn user email from db
        'Dim loggedInUserEmail = AuthData.User.Email

        Dim mandrillAPIKey = ConfigurationManager.AppSettings("MANDRILL_APIKey").ToString()
        Dim mandrillSender = ConfigurationManager.AppSettings("MANDRILL_SENDER").ToString()
        Dim mandrillHost = ConfigurationManager.AppSettings("MANDRILL_Host").ToString()
        Dim mandrillPort = ConfigurationManager.AppSettings("MANDRILL_Port")


        Dim orgProxy = New Common.OrganizationClient()
        Dim user = orgProxy.GetAccountEmail(AuthData.User.UserId).Data

        If user IsNot Nothing Then
            Dim loggedInUserEmail = user.Email
            Dim SmtpServer As New SmtpClient()
            SmtpServer.UseDefaultCredentials = False
            SmtpServer.Credentials = New Net.NetworkCredential(mandrillSender, mandrillAPIKey)
            SmtpServer.Port = mandrillPort
            SmtpServer.EnableSsl = False
            SmtpServer.Host = mandrillHost

            Dim selectedReport = ReportTypes.SelectedItem.Key
            Dim fromTime1 = FromTime.Value
            Dim toTime1 = ToTime.Value


            Try
                Dim mail = New MailMessage()
                mail.From = New MailAddress(mandrillSender)
                mail.To.Add(loggedInUserEmail.ToString())
                mail.Subject = "Report"
                mail.IsBodyHtml = False
                mail.Body = "Hi " + AuthData.User.FullName + "," & vbLf & vbLf & "Please find " + selectedReport + " Report for period " + fromTime1 + " to " + toTime1 + " in attachment."
                mail.Attachments.Add(attach)
                SmtpServer.Send(mail)
                MsgBox("Report has been sent successfully to " + loggedInUserEmail.ToString())
                Return True
            Catch ex As Exception
                'Exception
                MsgBox("Something went wrong, please try again.")
                Return False
            Finally
                ms.Close()
            End Try
        End If
        Return True
    End Function



    Private Sub EmailReport_btn_Click(sender As Object, e As EventArgs) Handles EmailReport_btn.Click
        Dim status = SendPDFReport()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class