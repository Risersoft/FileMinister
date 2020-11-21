Imports risersoft.shared.portable.Model

Public Class UserShareController
    Inherits LocalApiController(Of UserShareInfo, Int32, IUserShareRepository)


    ''' <summary>
    ''' Get UserShare By share id and user's account id
    ''' </summary>
    ''' <param name="repository"></param>
    ''' <remarks></remarks>
    Public Sub New(repository As IUserShareRepository)
        MyBase.New(repository)
    End Sub


End Class
