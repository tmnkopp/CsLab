Imports System.Net
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Linq

Public Class ValuesController
    Inherits ApiController
    Dim dictionary As New Dictionary(Of String, String)
    ' GET api/<controller>
    <HttpGet>
    <Route("api/values")>
    Public Function GetValues()
        dictionary.Add("bird", "20")
        dictionary.Add("frog", "1")
        dictionary.Add("snake", "10")
        dictionary.Add("fish", "2")
        Return Me.Json(dictionary)
    End Function

    ' GET api/<controller>/5
    Public Function GetValue(ByVal id As Integer) As String
        Return "value"
    End Function

    ' POST api/<controller>
    <HttpPost>
    <Route("api/values")>
    Public Sub PostValue(<FromBody()> ByVal value)
        Dim t As Testy = JObject.FromObject(value).ToObject(Of Testy)()
        Dim j = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(value)
        Dim x = value
    End Sub

    ' PUT api/<controller>/5
    Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

    End Sub

    ' DELETE api/<controller>/5
    Public Sub DeleteValue(ByVal id As Integer)

    End Sub
End Class

Public Class Testy
    Private _value As String
    Public Property value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property
End Class
