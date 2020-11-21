Imports CloudSync.Web.Controllers

Public Class HomeController
    Inherits BaseController

    Function Index() As ActionResult
        Me.SetViewBagProperty()
        Return View()
    End Function

    Function DeletedFiles() As ActionResult
        Me.SetViewBagProperty()
        Return View("DeletedFiles")
    End Function

End Class
