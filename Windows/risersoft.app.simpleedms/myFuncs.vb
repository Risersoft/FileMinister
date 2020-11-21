Public Class myFuncs
    Public Sub fncWOExplore(ByRef oView As clsWinView, ByVal PIDUnitID As Integer)
        Dim f As frmExplore

        f = New frmExplore
        f.InitViewPIDU(PIDUnitID)
        WinFormUtils.CentreForm(f, oView.fParentWin)

    End Sub
    Public Sub fncServerExplore(ByRef oView As clsWinView)
        Dim f As frmExplore

        f = New frmExplore
        f.InitViewServer()
        WinFormUtils.CentreForm(f, oView.fParentWin)

    End Sub
   
End Class
