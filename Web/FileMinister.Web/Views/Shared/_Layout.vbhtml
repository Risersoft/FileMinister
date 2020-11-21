<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    @*<title>@ViewBag.Title - File Minister</title>*@
    <title>File Minister</title>
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
    <link href="~/Content/ngDialog.css" rel="stylesheet" />
    <link href="~/Content/ngDialog-theme-default.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="anonymous">

    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" rel="stylesheet">
    <link href="~/Content/fileBrowser.css" rel="stylesheet">
    <link href="~/Content/Site.css" rel="stylesheet" />

    <link rel="stylesheet" href="https://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
</head>
<body ng-app="cloudSync">
    <!-- Begin page content -->
    <div class="container-fluid">
        @RenderBody()
    </div>

    @Scripts.Render("~/bundles/mainControllers")
    <script src="~/Scripts/dropzone.js" type="text/javascript"></script>
    <script src="~/Scripts/angular-md5.js"></script>
    @RenderSection("script", False)

</body>
</html>
