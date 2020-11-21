Imports FileMinister.Client.WinApp.Auth
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.cloud

Public Class SelectAgent
    Public Agent As New AgentMacAddressInfo

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles Me.Load

#If Not DEBUG Then
        lblMACAddressesLabel.Visible = False
        lbMACAddresses.Visible = False
#End If

#If DEBUG Then
        For Each item In CommonUtils.Helper.GetMacAddresses()
            lbMACAddresses.Items.Add(item)
        Next
#End If
        'lblMACAddress.Text = CommonUtils.Helper.getMacAddress()
    End Sub

    Private Sub btnNextAgentDetail_Click(sender As Object, e As EventArgs) Handles btnNextAgentDetail.Click
        If String.IsNullOrWhiteSpace(txtAgent.Text) Then 'Or String.IsNullOrWhiteSpace(txtMACAddress.Text) Then
            MessageBoxHelper.ShowWarningMessage("Agent and Secret Key are required")
            Exit Sub
        End If

        Using service As New AgentClient()
            'Dim result = service.ValidateAgentWithMAC(txtAgent.Text.Trim(), txtMACAddress.Text.Trim())
            Dim secretKeyEncrypted = CommonUtils.Helper.Encrypt(txtSecretKey.Text.Trim())
            Dim result = service.ValidateAgentWithMAC(txtAgent.Text.Trim(), CommonUtils.Helper.GetMacAddresses(), secretKeyEncrypted)
            If result.Data Is Nothing Then
                MessageBoxHelper.ShowErrorMessage("Invalid Agent or Secret Key")
                Exit Sub
            End If

            If (result.Data IsNot Nothing) Then


                Agent = result.Data
                Me.Close()

                AuthProvider.OnAgentValidated(Nothing, Agent)
            Else
                MessageBoxHelper.ShowErrorMessage("Oops! something went wrong. Please try again after sometime.")
            End If

        End Using
    End Sub



End Class