Imports System.Web.Services
Imports CyberBalance.CS.Core
Imports CyberBalance.VB.Core
Imports CyberBalance.VB.Web.UI
Imports Newtonsoft.Json

Public Class DBUtils
    Inherits System.Web.UI.Page

    ' Simple Request
    <WebMethod()>
    Public Shared Function GetDataTables(requests As Dictionary(Of String, DataRequest))
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim JsonDictOfDataTables = New DataResponseService() _
            .SetUser(_CAUser) _
            .ApplyRequest(requests) _
            .ApplyUrlEncryption(_UrlParams) _
            .GetResponse()

        Return JsonDictOfDataTables

    End Function

    'If you need to access the data
    <WebMethod()>
    Public Shared Function SprocRequest2(requests As Dictionary(Of String, DataRequest))
        Dim _CAUser As CAuser, _UrlParams As URLParms
        CBWebBase.Init(_CAUser, _UrlParams)

        Dim DictOfDataTables As Dictionary(Of String, DataTable) = New DataResponseService() _
            .SetUser(_CAUser) _
            .ApplyRequest(requests) _
            .ApplyUrlEncryption(_UrlParams) _
            .GetDataTables()

        For Each kv As KeyValuePair(Of String, DataTable) In DictOfDataTables
            Dim dataTable = DictOfDataTables(kv.Key)
            For Each dataRow As DataRow In dataTable.Rows
                'do stuff to data
            Next
        Next
        Dim JsonDictOfDataTables = JsonConvert.SerializeObject(DictOfDataTables)
        Return DictOfDataTables

    End Function
End Class



















'
'
'    <WebMethod()>
''    Public Shared Function PostData(user As UserFormViewModel)
''        Dim dt = New DataTable
''        dt.Columns.Add("col1")
''        Return JsonConvert.SerializeObject(dt)
''    End Function
'
'' Public Class UserFormViewModel
''     Public EmpId As String
''     Public EmpName As String
''     Public EmpDate As String
'' End Class





