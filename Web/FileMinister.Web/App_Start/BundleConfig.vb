Imports System.Web.Optimization

Public Module BundleConfig
    ' For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    Public Sub RegisterBundles(ByVal bundles As BundleCollection)


        bundles.Add(New ScriptBundle("~/bundles/mainControllers").Include("~/Scripts/jquery-3.3.1.min.js") _
                    .Include("~/scripts/angular.min.js") _
                    .Include("~/scripts/angular-sanitize.min.js") _
                    .Include("~/scripts/bootstrap.min.js") _
                    .Include("~/scripts/ngDialog.min.js") _
                    .Include("~/scripts/contextMenu.js") _
                    .Include("~/scripts/ag-grid.js") _
                    .Include("~/app/common/*.js") _
                    .Include("~/app/app.js") _
                    .Include("~/app/interfaces/*.js") _
                    .Include("~/app/services/*.js") _
                    .Include("~/app/controllers/*.js"))

        'BundleTable.EnableOptimizations = True

    End Sub
End Module

