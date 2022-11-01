Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Reflection
Imports BMSS_Sync_Program.Models

Public Class CURD
    Protected Shared ConString As String = ConfigurationManager.ConnectionStrings("Custom_CRM_DB_ConnectionString").ToString
    Protected Shared SAPConString As String = ConfigurationManager.ConnectionStrings("SAP_DB_ConnectionString").ToString
    Public Shared Function GetConnectionString(Optional ByVal Type As String = "Custom") As String

        Dim ReturnResult As String = String.Empty
        Try
            Try
                If Type = "Custom" Then
                    ReturnResult = ConString
                ElseIf Type = "SAP" Then
                    ReturnResult = SAPConString
                End If
            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
            End Try
            Return ReturnResult
        Catch ex As Exception
            Return ReturnResult
        End Try

    End Function
    Public Shared Function UpdateData(ByVal UQ As UpdateQuery) As Boolean
        Dim ReturnResult As Boolean = True
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If UQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)

                SqlCon.Open()

                Dim TableName As String = UQ._InputTable.GetType().Name
                Dim UpdateFields As New List(Of String)
                Dim UpdateParams As New List(Of String)
                Dim UpdateParamsValues As New List(Of String)
                Dim FilterParams As New List(Of String)
                Dim FilterParamsValues As New List(Of String)
                Dim Qry As String = "UPDATE  " & TableName & " SET "


                Dim InputProps As Type = UQ._InputTable.GetType
                For Each member As PropertyInfo In InputProps.GetProperties
                    If Not IsNothing(member.GetValue(UQ._InputTable, Nothing)) Then
                        Dim MemberValue As String = member.GetValue(UQ._InputTable, Nothing).ToString
                        If Not IsNothing(UQ._InputTableFiledsOperation) Then
                            Dim OperatorValue As String = member.GetValue(UQ._InputTableFiledsOperation, Nothing).ToString
                            Select Case OperatorValue
                                Case "-"
                                    UpdateFields.Add(member.Name & "=" & member.Name & "-@" & member.Name)
                                Case "+"
                                    UpdateFields.Add(member.Name & "=" & member.Name & "+@" & member.Name)
                                Case Else
                                    UpdateFields.Add(member.Name & "=@" & member.Name)
                            End Select
                        Else
                            UpdateFields.Add(member.Name & "=@" & member.Name)
                        End If

                        UpdateParams.Add("@" & member.Name)
                        UpdateParamsValues.Add(member.GetValue(UQ._InputTable, Nothing))
                    ElseIf member.Name.ToLower = "LastUpdateOn" Then
                        UpdateFields.Add(member.Name & "=@" & member.Name)
                        UpdateParams.Add("@" & member.Name)
                        UpdateParamsValues.Add(Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss"))
                    End If
                Next
                If UQ._HasWhereConditions = True Then

                    Dim FilterProps As Type = UQ._FilterTable.GetType
                    For Each member As PropertyInfo In FilterProps.GetProperties
                        If Not IsNothing(member.GetValue(UQ._FilterTable, Nothing)) Then
                            Dim MemberValue As String = member.GetValue(UQ._FilterTable, Nothing).ToString
                            FilterParams.Add("@Filter_" & member.Name)
                            FilterParamsValues.Add(member.GetValue(UQ._FilterTable, Nothing))
                        End If
                    Next

                    Qry = Qry & String.Join(",", UpdateFields.ToArray)

                    Qry = Qry & " WHERE "


                    If UQ._Conditions.Count > 0 Then
                        Dim ConditionsCount As Integer = 0
                        Dim pair As KeyValuePair(Of String, List(Of String))
                        For Each pair In UQ._Conditions
                            ' Display Key and Value.
                            If ConditionsCount > 0 And UQ._HasInBetweenConditions = True Then
                                Qry = Qry & UQ._InBetweenCondition
                            End If

                            If pair.Value.Count > 0 Then
                                Select Case pair.Key.ToString
                                    Case " AND ", " OR "
                                        Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                                End Select
                            End If

                            ConditionsCount = ConditionsCount + 1

                        Next
                    End If


                End If




                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each value As String In FilterParams
                        cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                        i = i + 1
                    Next
                    i = 0
                    For Each value As String In UpdateParams
                        cmd.Parameters.AddWithValue(value, UpdateParamsValues.Item(i))
                        i = i + 1
                    Next
                    cmd.ExecuteNonQuery()
                End Using

                Return ReturnResult
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    Public Shared Function InsertData(ByVal InputTable As Object, Optional ByVal HasPrimaryKey As Boolean = False, Optional ByVal PassedTableName As String = "") As Boolean
        Dim ReturnResult As Boolean = False
        Try

            Using SqlCon As New SqlConnection(ConString)

                SqlCon.Open()
                'Query Building - starts here

                Dim TableName As String = InputTable.GetType().Name


                If PassedTableName <> String.Empty Then
                    TableName = PassedTableName
                End If

                Dim Qry As String = "Insert into "
                Qry = Qry & TableName
                Dim QueryParams As New List(Of String)
                Dim SqlParams As New List(Of String)
                Dim SqlParamsValues As New List(Of String)


                Dim props As Type = InputTable.GetType
                For Each member As PropertyInfo In props.GetProperties
                    If member.Name <> "DocEntry" Or HasPrimaryKey = False Then
                        If Not IsNothing(member.GetValue(InputTable, Nothing)) Then
                            QueryParams.Add(member.Name)
                            SqlParams.Add("@" & member.Name)
                            SqlParamsValues.Add(member.GetValue(InputTable, Nothing).ToString)
                        ElseIf member.Name.ToLower = "createdon" Then
                            QueryParams.Add(member.Name)
                            SqlParams.Add("@" & member.Name)
                            SqlParamsValues.Add(Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss"))
                        End If
                    End If
                Next

                Qry = Qry & " (" & String.Join(",", QueryParams.ToArray) & ") "

                Qry = Qry & " Values "

                Qry = Qry & " (" & String.Join(",", SqlParams.ToArray) & ") "




                'Query Building - ends here


                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each Value As String In SqlParamsValues
                        cmd.Parameters.AddWithValue(SqlParams(i), Value)
                        i = i + 1
                    Next
                    cmd.ExecuteNonQuery()

                End Using

                ReturnResult = True


            End Using

            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
    Public Shared Function InsertDataTransaction(ByVal InputTable As Object, ByRef Connection As SqlConnection, ByRef SqlTrans As SqlTransaction, ByRef PrimaryKeyValue As String, Optional ByVal HasPrimaryKey As Boolean = False) As Boolean
        Dim ReturnResult As Boolean = False
        Try




            'Query Building - starts here
            Dim TableName As String = InputTable.GetType().Name

            Dim Qry As String = "Insert into "
            Qry = Qry & TableName
            Dim QueryParams As New List(Of String)
            Dim SqlParams As New List(Of String)
            Dim SqlParamsValues As New List(Of String)


            Dim props As Type = InputTable.GetType
            For Each member As PropertyInfo In props.GetProperties
                If member.Name <> "DocEntry" Or HasPrimaryKey = False Then
                    If Not IsNothing(member.GetValue(InputTable, Nothing)) Then
                        QueryParams.Add(member.Name)
                        SqlParams.Add("@" & member.Name)
                        SqlParamsValues.Add(member.GetValue(InputTable, Nothing).ToString)
                    ElseIf member.Name.ToLower = "createdon" Then
                        QueryParams.Add(member.Name)
                        SqlParams.Add("@" & member.Name)
                        SqlParamsValues.Add(Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss"))
                    End If
                End If
            Next

            Qry = Qry & " (" & String.Join(",", QueryParams.ToArray) & ") "

            If HasPrimaryKey Then
                Qry = Qry & " OUTPUT INSERTED." & "Id "
            End If

            Qry = Qry & " Values "

            Qry = Qry & " (" & String.Join(",", SqlParams.ToArray) & ") "




            'Query Building - ends here


            Using cmd As New SqlCommand(Qry)
                cmd.Connection = Connection
                cmd.Transaction = SqlTrans

                Dim i As Integer = 0
                For Each Value As String In SqlParamsValues
                    cmd.Parameters.AddWithValue(SqlParams(i), Value)
                    i = i + 1
                Next
                If HasPrimaryKey Then
                    Dim PrimaryKeyValueReader As SqlDataReader = cmd.ExecuteReader()
                    If PrimaryKeyValueReader.Read Then
                        PrimaryKeyValue = PrimaryKeyValueReader("DocEntry").ToString
                    End If
                    PrimaryKeyValueReader.Close()
                Else
                    cmd.ExecuteNonQuery()
                End If

            End Using

            ReturnResult = True




            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
    Public Shared Function UpdateDataTransaction(ByVal UQ As UpdateQuery, ByRef Connection As SqlConnection, ByRef SqlTrans As SqlTransaction, ByRef PrimaryKeyValue As String, Optional ByVal HasPrimaryKey As Boolean = False) As Boolean
        Dim ReturnResult As Boolean = True
        Try

            Dim TableName As String = UQ._InputTable.GetType().Name
            Dim UpdateFields As New List(Of String)
            Dim UpdateParams As New List(Of String)
            Dim UpdateParamsValues As New List(Of String)
            Dim FilterParams As New List(Of String)
            Dim FilterParamsValues As New List(Of String)
            Dim Qry As String = "UPDATE  " & TableName & " SET "


            Dim InputProps As Type = UQ._InputTable.GetType
            For Each member As PropertyInfo In InputProps.GetProperties
                If Not IsNothing(member.GetValue(UQ._InputTable, Nothing)) Then
                    Dim MemberValue As String = member.GetValue(UQ._InputTable, Nothing).ToString
                    If Not IsNothing(UQ._InputTableFiledsOperation) Then
                        Dim OperatorValue As String = member.GetValue(UQ._InputTableFiledsOperation, Nothing).ToString
                        Select Case OperatorValue
                            Case "-"
                                UpdateFields.Add(member.Name & "=" & member.Name & "-@" & member.Name)
                            Case "+"
                                UpdateFields.Add(member.Name & "=" & member.Name & "+@" & member.Name)
                            Case Else
                                UpdateFields.Add(member.Name & "=@" & member.Name)
                        End Select
                    Else
                        UpdateFields.Add(member.Name & "=@" & member.Name)
                    End If


                    UpdateParams.Add("@" & member.Name)
                    UpdateParamsValues.Add(member.GetValue(UQ._InputTable, Nothing))
                ElseIf member.Name.ToLower = "lastupdateon" Then
                    UpdateFields.Add(member.Name & "=@" & member.Name)
                    UpdateParams.Add("@" & member.Name)
                    UpdateParamsValues.Add(Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss"))
                End If
            Next
            If UQ._HasWhereConditions = True Then

                Dim FilterProps As Type = UQ._FilterTable.GetType
                For Each member As PropertyInfo In FilterProps.GetProperties
                    If Not IsNothing(member.GetValue(UQ._FilterTable, Nothing)) Then
                        Dim MemberValue As String = member.GetValue(UQ._FilterTable, Nothing).ToString
                        FilterParams.Add("@Filter_" & member.Name)
                        FilterParamsValues.Add(member.GetValue(UQ._FilterTable, Nothing))
                    End If
                Next

                Qry = Qry & String.Join(",", UpdateFields.ToArray)

                If HasPrimaryKey Then
                    Qry = Qry & " OUTPUT INSERTED." & "Id "
                End If

                Qry = Qry & " WHERE "


                If UQ._Conditions.Count > 0 Then
                    Dim ConditionsCount As Integer = 0
                    Dim pair As KeyValuePair(Of String, List(Of String))
                    For Each pair In UQ._Conditions
                        ' Display Key and Value.
                        If ConditionsCount > 0 And UQ._HasInBetweenConditions = True Then
                            Qry = Qry & UQ._InBetweenCondition
                        End If

                        If pair.Value.Count > 0 Then
                            Select Case pair.Key.ToString
                                Case " AND ", " OR "
                                    Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                            End Select
                        End If

                        ConditionsCount = ConditionsCount + 1

                    Next
                End If


            End If




            Using cmd As New SqlCommand(Qry)
                cmd.Connection = Connection
                cmd.Transaction = SqlTrans
                Dim i As Integer = 0
                For Each value As String In FilterParams
                    cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                    i = i + 1
                Next
                i = 0
                For Each value As String In UpdateParams
                    cmd.Parameters.AddWithValue(value, UpdateParamsValues.Item(i))
                    i = i + 1
                Next
                If HasPrimaryKey Then
                    Dim PrimaryKeyValueReader As SqlDataReader = cmd.ExecuteReader()
                    If PrimaryKeyValueReader.Read Then
                        PrimaryKeyValue = PrimaryKeyValueReader("DocEntry").ToString
                    End If
                    PrimaryKeyValueReader.Close()
                Else
                    cmd.ExecuteNonQuery()
                End If

            End Using

            Return ReturnResult

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    Public Shared Function DeleteDataTransaction(ByVal DQ As DeleteQuery, ByRef Connection As SqlConnection, ByRef SqlTrans As SqlTransaction, ByRef PrimaryKeyValue As String, Optional ByVal HasPrimaryKey As Boolean = False) As Boolean
        Dim ReturnResult As Boolean = True
        Try
            Dim TableName As String = DQ._InputTable.GetType().Name

            Dim QueryParams As New List(Of String)
            Dim FilterParams As New List(Of String)
            Dim FilterParamsValues As New List(Of String)
            Dim Qry As String = "DELETE  "


            Dim props As Type = DQ._InputTable.GetType
            For Each member As PropertyInfo In props.GetProperties


                If Not IsNothing(member.GetValue(DQ._InputTable, Nothing)) Then
                    FilterParams.Add("@" & member.Name)
                    FilterParamsValues.Add(member.GetValue(DQ._InputTable, Nothing).ToString)
                End If
            Next



            Qry = Qry & " FROM  " & TableName

            If HasPrimaryKey Then
                Qry = Qry & " OUTPUT DELETED." & "Id "
            End If

            If DQ._HasWhereConditions = True Then
                Qry = Qry & " WHERE "


                If DQ._Conditions.Count > 0 Then
                    Dim ConditionsCount As Integer = 0
                    Dim pair As KeyValuePair(Of String, List(Of String))
                    For Each pair In DQ._Conditions
                        ' Display Key and Value.
                        If ConditionsCount > 0 And DQ._HasInBetweenConditions = True Then
                            Qry = Qry & DQ._InBetweenCondition
                        End If

                        If pair.Value.Count > 0 Then
                            Select Case pair.Key.ToString
                                Case " AND ", " OR "
                                    Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                            End Select
                        End If

                        ConditionsCount = ConditionsCount + 1

                    Next
                End If


            End If




            Using cmd As New SqlCommand(Qry)
                cmd.Connection = Connection
                cmd.Transaction = SqlTrans
                Dim i As Integer = 0
                For Each value As String In FilterParams
                    cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                    i = i + 1
                Next
                If HasPrimaryKey Then
                    Dim PrimaryKeyValueReader As SqlDataReader = cmd.ExecuteReader()
                    If PrimaryKeyValueReader.Read Then
                        PrimaryKeyValue = PrimaryKeyValueReader("DocEntry").ToString
                    End If
                    PrimaryKeyValueReader.Close()
                Else
                    cmd.ExecuteNonQuery()
                End If
            End Using

            Return ReturnResult

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    Public Shared Function SelectAllData(ByVal SQ As SelectQuery) As DataTable
        Dim ReturnResult As New DataTable
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If SQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)

                SqlCon.Open()
                Dim TableName As String = SQ._InputTable.GetType().Name

                Dim QueryParams As New List(Of String)
                Dim FilterParams As New List(Of String)
                Dim FilterParamsValues As New List(Of String)
                Dim Qry As String = "SELECT "

                If Not IsNothing(SQ._TopRecord) Then
                    If IsNumeric(SQ._TopRecord) Then
                        If SQ._TopRecord > 0 Then
                            Qry = Qry & " Top " & SQ._TopRecord & "  "
                        End If
                    End If
                End If

                Dim props As Type = SQ._InputTable.GetType
                For Each member As PropertyInfo In props.GetProperties
                    QueryParams.Add(member.Name)


                    If Not IsNothing(member.GetValue(SQ._InputTable, Nothing)) Then
                        FilterParams.Add("@" & member.Name)
                        FilterParamsValues.Add(member.GetValue(SQ._InputTable, Nothing).ToString)
                    End If
                Next

                Qry = Qry & String.Join(",", QueryParams.ToArray)

                Qry = Qry & " FROM  " & TableName

                If SQ._HasWhereConditions = True Then
                    Qry = Qry & " WHERE "


                    If SQ._Conditions.Count > 0 Then
                        Dim ConditionsCount As Integer = 0
                        Dim pair As KeyValuePair(Of String, List(Of String))
                        For Each pair In SQ._Conditions
                            ' Display Key and Value.
                            If ConditionsCount > 0 And SQ._HasInBetweenConditions = True Then
                                Qry = Qry & SQ._InBetweenCondition
                            End If

                            If pair.Value.Count > 0 Then
                                Select Case pair.Key.ToString
                                    Case " AND ", " OR "
                                        Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                                End Select
                            End If

                            ConditionsCount = ConditionsCount + 1

                        Next
                    End If


                End If

                If Not IsNothing(SQ._OrderBy) Then
                    If SQ._OrderBy <> String.Empty Then
                        Qry = Qry & " ORDER BY " & SQ._OrderBy
                    End If
                End If


                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each value As String In FilterParams
                        cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                        i = i + 1
                    Next
                    Dim SqlAdap As New SqlDataAdapter(cmd)
                    SqlAdap.Fill(ReturnResult)
                    SqlAdap.Dispose()
                End Using

                Return ReturnResult
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
    Public Shared Function CustomQueryGetData(ByVal CQ As CustomQuery) As DataTable
        Dim ReturnResult As New DataTable
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If CQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)
                SqlCon.Open()
                Dim Qry As String = CQ._InputQuery
                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    If Not IsNothing(CQ._Parameters) Then
                        Dim Params = CQ._Parameters
                        For Each iKey As String In Params.Keys
                            Dim ParamValue As String = Params(iKey)
                            cmd.Parameters.AddWithValue(iKey, ParamValue)
                        Next
                    End If

                    Dim SqlAdap As New SqlDataAdapter(cmd)
                    SqlAdap.Fill(ReturnResult)
                    SqlAdap.Dispose()
                End Using
            End Using

            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Public Shared Function CustomQueryUpdateData(ByVal CQ As CustomQuery) As Boolean
        Dim ReturnResult As Boolean = True
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If CQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)
                SqlCon.Open()
                Dim Qry As String = CQ._InputQuery
                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    If Not IsNothing(CQ._Parameters) Then
                        Dim Params = CQ._Parameters
                        For Each iKey As String In Params.Keys
                            Dim ParamValue As String = Params(iKey)
                            cmd.Parameters.AddWithValue(iKey, ParamValue)
                        Next
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    Public Shared Function DeleteData(ByVal DQ As DeleteQuery) As Boolean
        Dim ReturnResult As Boolean = True
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If DQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)

                SqlCon.Open()
                Dim TableName As String = DQ._InputTable.GetType().Name


                Dim FilterParams As New List(Of String)
                Dim FilterParamsValues As New List(Of String)
                Dim Qry As String = "DELETE "


                Dim props As Type = DQ._InputTable.GetType
                For Each member As PropertyInfo In props.GetProperties
                    If Not IsNothing(member.GetValue(DQ._InputTable, Nothing)) Then
                        FilterParams.Add("@" & member.Name)
                        FilterParamsValues.Add(member.GetValue(DQ._InputTable, Nothing).ToString)
                    End If
                Next



                Qry = Qry & " FROM  " & TableName

                If DQ._HasWhereConditions = True Then
                    Qry = Qry & " WHERE "


                    If DQ._Conditions.Count > 0 Then
                        Dim ConditionsCount As Integer = 0
                        Dim pair As KeyValuePair(Of String, List(Of String))
                        For Each pair In DQ._Conditions
                            ' Display Key and Value.
                            If ConditionsCount > 0 And DQ._HasInBetweenConditions = True Then
                                Qry = Qry & DQ._InBetweenCondition
                            End If

                            If pair.Value.Count > 0 Then
                                Select Case pair.Key.ToString
                                    Case " AND ", " OR "
                                        Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                                End Select
                            End If

                            ConditionsCount = ConditionsCount + 1

                        Next
                    End If


                End If




                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each value As String In FilterParams
                        cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                        i = i + 1
                    Next
                    cmd.ExecuteNonQuery()
                End Using

                Return ReturnResult
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    Public Shared Function SelectSpecificFieldsData(ByVal SQ As SelectQuery) As DataTable
        Dim ReturnResult As New DataTable
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If SQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)

                SqlCon.Open()

                Dim TableName As String = SQ._InputTable.GetType().Name

                Dim QueryParams As New List(Of String)
                Dim FilterParams As New List(Of String)
                Dim FilterParamsValues As New List(Of String)
                Dim Qry As String = "SELECT "

                If Not IsNothing(SQ._TopRecord) Then
                    If IsNumeric(SQ._TopRecord) Then
                        If SQ._TopRecord > 0 Then
                            Qry = Qry & " Top " & SQ._TopRecord & "  "
                        End If
                    End If
                End If

                Dim props As Type = SQ._InputTable.GetType
                For Each member As PropertyInfo In props.GetProperties



                    If Not IsNothing(member.GetValue(SQ._InputTable, Nothing)) Then
                        Dim MemberValue As String = member.GetValue(SQ._InputTable, Nothing).ToString
                        If MemberValue <> "?" Then
                            FilterParams.Add("@" & member.Name)
                            FilterParamsValues.Add(member.GetValue(SQ._InputTable, Nothing).ToString)
                        Else
                            QueryParams.Add(member.Name)
                        End If

                    End If
                Next

                Qry = Qry & String.Join(",", QueryParams.ToArray)

                Qry = Qry & " FROM  " & TableName

                If SQ._HasWhereConditions = True Then
                    Qry = Qry & " WHERE "


                    If SQ._Conditions.Count > 0 Then
                        Dim ConditionsCount As Integer = 0
                        Dim pair As KeyValuePair(Of String, List(Of String))
                        For Each pair In SQ._Conditions
                            ' Display Key and Value.
                            If ConditionsCount > 0 And SQ._HasInBetweenConditions = True Then
                                Qry = Qry & SQ._InBetweenCondition
                            End If

                            If pair.Value.Count > 0 Then
                                Select Case pair.Key.ToString
                                    Case " AND ", " OR "
                                        Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                                End Select
                            End If

                            ConditionsCount = ConditionsCount + 1

                        Next
                    End If


                End If

                If Not IsNothing(SQ._OrderBy) Then
                    If SQ._OrderBy <> String.Empty Then
                        Qry = Qry & " ORDER BY " & SQ._OrderBy
                    End If
                End If

                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each value As String In FilterParams
                        cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                        i = i + 1
                    Next
                    Dim SqlAdap As New SqlDataAdapter(cmd)
                    SqlAdap.Fill(ReturnResult)
                    SqlAdap.Dispose()
                End Using

            End Using
            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
    Public Shared Function SelectAllDataComplexCondition(ByVal SQ As ComplexSelectQuery) As DataTable
        Dim ReturnResult As New DataTable
        Try
            Dim ConnectionStringToUse As String = String.Empty
            If SQ._DB = "SAP" Then
                ConnectionStringToUse = SAPConString
            Else
                ConnectionStringToUse = ConString
            End If
            Using SqlCon As New SqlConnection(ConnectionStringToUse)

                SqlCon.Open()
                Dim TableName As String = SQ._InputTable(0).GetType().Name

                Dim QueryParams As New List(Of String)
                Dim FilterParams As New List(Of String)
                Dim FilterParamsValues As New List(Of String)
                Dim Qry As String = "SELECT "

                If Not IsNothing(SQ._TopRecord) Then
                    If IsNumeric(SQ._TopRecord) Then
                        If SQ._TopRecord > 0 Then
                            Qry = Qry & " Top " & SQ._TopRecord & "  "
                        End If
                    End If
                End If

                Dim props As Type = SQ._InputTable(0).GetType
                Dim j As Integer = 0
                For Each Obj In SQ._InputTable
                    For Each member As PropertyInfo In props.GetProperties
                        If j = 0 Then
                            QueryParams.Add(member.Name)
                            If Not IsNothing(member.GetValue(SQ._InputTable(j), Nothing)) Then
                                FilterParams.Add("@" & member.Name)
                                FilterParamsValues.Add(member.GetValue(SQ._InputTable(j), Nothing).ToString)
                            End If
                        Else
                            If Not IsNothing(member.GetValue(SQ._InputTable(j), Nothing)) Then
                                FilterParams.Add("@" & member.Name & j)
                                FilterParamsValues.Add(member.GetValue(SQ._InputTable(j), Nothing).ToString)
                            End If
                        End If

                    Next
                    j = j + 1
                Next


                Qry = Qry & String.Join(",", QueryParams.ToArray)

                Qry = Qry & " FROM  " & TableName

                If SQ._HasWhereConditions = True Then
                    Qry = Qry & " WHERE "


                    If SQ._Conditions.Count > 0 Then
                        Dim ConditionsCount As Integer = 0
                        Dim pair As KeyValuePair(Of String, List(Of String))
                        For Each pair In SQ._Conditions
                            ' Display Key and Value.
                            If ConditionsCount > 0 And SQ._HasInBetweenConditions = True Then
                                Qry = Qry & SQ._InBetweenCondition
                            End If

                            If pair.Value.Count > 0 Then
                                Select Case pair.Key.ToString
                                    Case " AND ", " OR "
                                        Qry = Qry & " ( " & String.Join(pair.Key, pair.Value.ToArray) & " )"
                                End Select
                            End If

                            ConditionsCount = ConditionsCount + 1

                        Next
                    End If


                End If

                If Not IsNothing(SQ._OrderBy) Then
                    If SQ._OrderBy <> String.Empty Then
                        Qry = Qry & " ORDER BY " & SQ._OrderBy
                    End If
                End If


                Using cmd As New SqlCommand(Qry)
                    cmd.Connection = SqlCon
                    Dim i As Integer = 0
                    For Each value As String In FilterParams
                        cmd.Parameters.AddWithValue(value, FilterParamsValues.Item(i))
                        i = i + 1
                    Next
                    Dim SqlAdap As New SqlDataAdapter(cmd)
                    SqlAdap.Fill(ReturnResult)
                    SqlAdap.Dispose()
                End Using

                Return ReturnResult
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
#Region "App Specific functions "
    Public Shared Function GetDocNum(ByVal TableName As String, ByVal DocEntry As String) As String
        Dim DocNum As String = String.Empty
        Try


            Using SqlCon As New SqlConnection(SAPConString)
                Try

                    SqlCon.Open()
                    Dim Qry As String = "SELECT DocNum FROM  " & TableName & " WHERE DocEntry=@Value"

                    Using cmd As New SqlCommand(Qry)

                        cmd.Connection = SqlCon
                        cmd.Parameters.AddWithValue("@Value", DocEntry)
                        DocNum = cmd.ExecuteScalar()

                        cmd.Dispose()
                    End Using
                Catch ex As Exception
                    AppSpecificFunc.WriteLog(ex)
                End Try
            End Using

            Return DocNum
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return DocNum
        End Try
    End Function
    Public Shared Function GetNumber(ByVal TableName As String, ByVal DocEntry As String) As String
        Dim DocNum As String = String.Empty
        Try


            Using SqlCon As New SqlConnection(SAPConString)
                Try

                    SqlCon.Open()
                    Dim Qry As String = "SELECT Number FROM  " & TableName & " WHERE BatchNum=@Value"

                    Using cmd As New SqlCommand(Qry)

                        cmd.Connection = SqlCon
                        cmd.Parameters.AddWithValue("@Value", DocEntry)
                        DocNum = cmd.ExecuteScalar()

                        cmd.Dispose()
                    End Using
                Catch ex As Exception
                    AppSpecificFunc.WriteLog(ex)
                End Try
            End Using

            Return DocNum
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return DocNum
        End Try
    End Function
    Public Shared Function CheckFieldValueExist(ByVal TableName As String, ByVal ColumnName As String, ByVal Value As String, ByRef ErrMsg As String) As Boolean
        Dim ReturnResult As Boolean = False
        Try


            Using SqlCon As New SqlConnection(ConString)


                SqlCon.Open()
                Dim Qry As String = "SELECT " & ColumnName & " FROM  " & TableName & " WHERE LOWER(" & ColumnName & ")=@Value"

                Using cmd As New SqlCommand(Qry)
                    Dim DR As SqlDataReader
                    cmd.Connection = SqlCon
                    cmd.Parameters.AddWithValue("@Value", Value.ToLower)
                    DR = cmd.ExecuteReader()
                    If DR.Read Then
                        ReturnResult = True
                    Else
                        ReturnResult = False
                    End If
                    DR.Close()
                End Using

            End Using

            Return ReturnResult
        Catch ex As Exception
            ErrMsg = ex.Message
            AppSpecificFunc.WriteLog(ex)
            Return ReturnResult
        End Try
    End Function
#End Region



End Class
