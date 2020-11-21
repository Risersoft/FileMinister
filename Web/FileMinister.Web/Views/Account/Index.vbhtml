@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code

@section script
    <script>
        //var access_token = '@Me.ViewBag.access_token';
        //var refresh_token = '@Me.ViewBag.refresh_token';
        var localUrl = '@Me.ViewBag.localUrl';
        //localStorage["access_token"] = access_token;
        //localStorage["refresh_token"] = refresh_token;
        window.location.href = localUrl
    </script>
End Section
