Imports System.Web.Http
Imports System.Web.Optimization

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started

        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        'WebApiConfig.Register(Http.GlobalConfiguration.Configuration)
        Http.GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)

        Dim JsConfigFile = Server.MapPath("~/Scripts/core/config.js")
        Dim script = "
            export const environment = {
                production: false,
                baseUrl: '/' 
            };
            "
        'System.IO.File.WriteAllText(JsConfigFile, script)

    End Sub
End Class