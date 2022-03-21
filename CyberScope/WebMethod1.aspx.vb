Imports System.Runtime.CompilerServices
Imports System.Web.Services
Imports Newtonsoft.Json

Public Class WebMethod1
    Inherits System.Web.UI.Page

    <WebMethod()>
    Public Shared Function RequestData(request As DataRequest)
        Dim dt = New DataTable
        Return JsonConvert.SerializeObject(dt)
    End Function

    <WebMethod()>
    Public Shared Function PostData(emp As Employee)

        Dim dt = New DataTable
        dt.Columns.Add("col1")
        Return JsonConvert.SerializeObject(dt)
    End Function
End Class
Public Class Employee
    Public EmpId As String
    Public EmpName As String
    Public EmpDate As String
End Class
Public Class DataRequest
    Public SPROC As String
    Public PARMS As Dictionary(Of String, String)
    Public Sub DataRequest()
        PARMS = New Dictionary(Of String, String)
    End Sub
End Class



