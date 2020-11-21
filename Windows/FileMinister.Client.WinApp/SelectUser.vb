Imports FileMinister.Client.Common

Public Class SelectUser

    Public Property SelectedValue As DataGridViewRow

    Public Property accountName As String

    Public Sub New(AccountName As String)



        ' This call is required by the designer.
        InitializeComponent()

        Me.accountName = AccountName

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub rtbSelectUser_TextChanged(sender As Object, e As EventArgs) Handles rtbSelectUser.TextChanged
        Dim elementCount As Integer = rtbSelectUser.TextLength
        btnCheckNames.Enabled = rtbSelectUser.TextLength
        btnOK.Enabled = rtbSelectUser.TextLength
    End Sub

    Private Sub btnCheckNames_Click(sender As Object, e As EventArgs) Handles btnCheckNames.Click

        Dim dt As New DataTable()
        dt.Columns.AddRange(New DataColumn(3) {New DataColumn("Key", GetType(String)), New DataColumn("Value", GetType(String)), New DataColumn("Type", GetType(String)), New DataColumn("TypeName", GetType(String))})

        Using UserProxy = New OrganizationClient()
            Dim userData = UserProxy.GetMyAccountGroupUser(rtbSelectUser.Text, accountName)

            For Each item In userData.Data
                If (item("typeId") = 1) Then
                    dt.Rows.Add(item("id"), item("name"), item("typeId"), "User")
                Else
                    dt.Rows.Add(item("id"), item("name"), item("typeId"), "Group")
                End If
            Next
            'TO CHECK NEW USER
            'dt.Rows.Add("100", "New User 1", "1", "User")

        End Using



        '       dt.Rows.Add("100", "New User 1", "1", "User")
        'dt.Rows.Add("200", "New User 1", "1", "User")
        'dt.Rows.Add("300", "New Group 1", "2", "Group")

        Me.dgvUG.DataSource = dt
        Me.dgvUG.Columns(0).Visible = False
        Me.dgvUG.Columns(2).Visible = False

        Me.dgvUG.Columns(1).Width = 150

        'Dim dt As New DataTable()
        'dt.Columns.AddRange(New DataColumn(1) {New DataColumn("Key", GetType(String)), New DataColumn("Value", GetType(String))})
        'dt.Rows.Add("Author", "Abhishek")
        'dt.Rows.Add("Title", "NA")
        'dt.Rows.Add("Subject", "Test")
        'dt.Rows.Add("Categories", "Cloud")
        'dt.Rows.Add("Comments", "Test Metadata")

        'lstViewSearchResult.Visible = True
        'lstViewSearchResult.Columns.Add("Key", 100, HorizontalAlignment.Left)
        'lstViewSearchResult.Columns.Add("Value", 100, HorizontalAlignment.Left)


        'For inc = 0 To dt.Rows.Count - 1
        '    Dim str(5) As String
        '    Dim listitem As New ListViewItem
        '    str(0) = dt.Rows(inc).Item(0).ToString()
        '    str(1) = dt.Rows(inc).Item(1).ToString()
        '    listitem = New ListViewItem(str)
        '    lstViewSearchResult.Items.Add(listitem)
        'Next inc


    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If (dgvUG.SelectedRows.Count > 0) Then
            Dim selRow = dgvUG.SelectedRows(0)
            Me.SelectedValue = selRow
            Me.Close()
        Else
            Me.Close()
        End If
    End Sub

End Class