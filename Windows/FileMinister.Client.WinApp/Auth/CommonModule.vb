Imports System.Text.RegularExpressions
Imports FileMinister.Client.Common

Module CommonModule
    Public UserId As Integer
    Public AccountId As Integer
    Public SessionId As String
    Public access_token As String
    Public LoggedinUser As Users
    Public LoginOtp As String
    Public ProviderKey As String


    Public Function EmailValidation(ByVal ctxt As System.Windows.Forms.TextBox) As Boolean
        If ctxt.Text.Trim <> "" Then
            Dim rex As Match = Regex.Match(Trim(ctxt.Text), "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,3})$", RegexOptions.IgnoreCase)
            If rex.Success = False Then
                MessageBoxHelper.ShowInfoMessage("Please Enter a valid Email-Address")
                ctxt.Focus()
                Return False
            Else
                Return True
            End If
        Else
            Return True
        End If
    End Function


    'Public Function MaximpriseAuthServiceLoggedin(userName As String, Optional password As String = "") As Users

    '    Dim postData = "grant_type=password&username=" + userName.Trim() + "&password=" + password.Trim()
    '    Dim strUrl As String = FileMinister.Common.Constants.AUTH_URI & "/api/token"
    '    Dim myWebClient As New WebClient()
    '    myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
    '    If (String.IsNullOrEmpty(password)) Then
    '        myWebClient.Headers.Add("User-Agent", "social")
    '    Else
    '        myWebClient.Headers.Add("User-Agent", "self")
    '    End If

    '    Dim responseArray = myWebClient.UploadData(strUrl, "POST", System.Text.Encoding.ASCII.GetBytes(postData))
    '    Dim response = Encoding.ASCII.GetString(responseArray)
    '    Dim user = JsonConvert.DeserializeObject(Of Users)(response)
    '    If (Not IsNothing(user.ErrorMessage)) Then
    '        MessageBox.Show(LoggedinUser.ErrorMessage)
    '        Return Nothing
    '    End If
    '    LoggedinUser = user
    '    user.Id = user.UserId
    '    access_token = user.access_token
    '    SessionId = user.SessionId
    '    UserId = user.UserId
    '    AccountId = user.AccountId
    '    user.AccessToken = user.access_token
    '    AuthData.AccessToken = user.access_token
    '    AuthProvider.OnUserAuthenticated(Nothing, user)
    '    Return user
    'End Function

    Public Async Function AuthenticateUserAsync(userName As String, password As String) As Task(Of Users)

        Using client As New Client.Common.AccountClient()
            Dim authResult = Await client.AuthenticateUserAsync(userName, password)
            If authResult.StatusCode = 200 Then
                Dim user = authResult.Data
                LoggedinUser = user
                user.Id = user.UserId
                access_token = user.access_token
                SessionId = user.SessionId
                UserId = user.UserId
                AccountId = user.AccountId
                user.AccessToken = user.access_token
                Return user
            End If
            Return Nothing
        End Using
    End Function

End Module

