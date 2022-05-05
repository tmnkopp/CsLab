Imports System.Web
Imports System.Web.Services
Imports Newtonsoft.Json

Public Class Handler1
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "application/json"
        context.Response.Write(JsonConvert.SerializeObject(New With {Key .response = "Response"}))

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class