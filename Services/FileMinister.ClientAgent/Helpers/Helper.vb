Imports System.Net.NetworkInformation
Imports System.Security.Principal
Imports System.Text.RegularExpressions

Public Class Helper
    Public Shared Function GetMacAddresses() As List(Of String)
        Dim nics = NetworkInterface.GetAllNetworkInterfaces().Where(Function(p) p.OperationalStatus = OperationalStatus.Up)
        Dim data = nics.Select(Function(p) p.GetPhysicalAddress().ToString()).Where(Function(p) Not String.IsNullOrWhiteSpace(p)).Select(Function(p) Regex.Replace(p, ".{2}", "-$0").Substring(1)).ToList()
        Return data
    End Function

    Public Shared Function GetUserSID(userName As String, domainName As String) As String
        Dim account As New NTAccount(domainName, userName)
        Dim sid As SecurityIdentifier = CType(account.Translate(GetType(SecurityIdentifier)), SecurityIdentifier)
        Return sid.ToString()
    End Function
End Class
