﻿Namespace Models
    Public Class DeleteQuery

        Public Property _InputTable() As Object
        Public Property _HasWhereConditions() As Boolean
        Public Property _Conditions() As Dictionary(Of String, List(Of String))
        Public Property _HasInBetweenConditions() As Boolean
        Public Property _InBetweenCondition() As String
        Public Property _DB() As String
    End Class
End Namespace