Namespace Models
    Public Class SelectQuery
        Private TopRecord As Integer

        Public Sub New()
            TopRecord = 0
        End Sub
        Public Property _InputTable() As Object
        Public Property _HasWhereConditions() As Boolean
        Public Property _Conditions() As Dictionary(Of String, List(Of String))
        Public Property _HasInBetweenConditions() As Boolean
        Public Property _InBetweenCondition() As String
        Public Property _OrderBy() As String
        Public Property _TopRecord() As Integer
            Get
                Return TopRecord
            End Get
            Set(ByVal value As Integer)
                If IsNothing(value) Then
                    TopRecord = 0
                Else
                    TopRecord = value
                End If
            End Set
        End Property
        Public Property _DB() As String
    End Class
End Namespace