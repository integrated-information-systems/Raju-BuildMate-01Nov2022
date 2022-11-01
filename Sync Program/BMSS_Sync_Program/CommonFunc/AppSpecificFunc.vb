Imports System.Reflection

Namespace Models
    Public Class AppSpecificFunc
        Public Shared Function GetSyncLog() As DataTable
            Try

                Dim ResultDataTable As New DataTable
                '**********************Query Builder Function *****************
                Dim SyncLogTable As New SyncLog

                ' INPUT FOR Query Builder
                Dim SQB As New SelectQuery
                SQB._InputTable = SyncLogTable
                SQB._DB = "Custom"
                SQB._HasInBetweenConditions = False
                SQB._HasWhereConditions = False
                SQB._OrderBy = "CreatedOn DESC"

                ResultDataTable = CURD.SelectAllData(SQB)
                '**********************Query Builder Function *****************                
                Return ResultDataTable

            Catch ex As Exception
                WriteLog(ex)
                Return Nothing
            End Try
        End Function
        Public Shared Function GetProcessesValue(ByVal InputStatus As String) As Processes
            Dim ReturnResult As Integer = -1
            Try
                InputStatus = InputStatus.Replace(" ", "_")
                InputStatus = "Process_" & InputStatus
                For Each Process As Processes In System.Enum.GetValues(GetType(Processes))
                    If Process.ToString.IndexOf(InputStatus) > -1 Then
                        ReturnResult = Process
                    End If
                Next
                Return ReturnResult
            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
                Return ReturnResult
            End Try

        End Function
        Public Shared Function GetProcessesName(ByRef InputValue As Processes) As String
            Dim ReturnResult As String = String.Empty
            Try
                ReturnResult = InputValue.ToString.Replace("Process_", "")
                ReturnResult = ReturnResult.Replace("_", " ")
                Return ReturnResult
            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
                Return ReturnResult
            End Try
        End Function
        Public Shared Function UpdateSyncStaus(TableName As String, SyncedToSAP As Boolean, SAPDocNum As String, DocEntry As String, SyncRemarks As String, Optional SAPPaymentDocNum As String = "") As Boolean
            Dim ReturnResult As Boolean = True
            Try
                Dim ResultDataTable As New DataTable
                Dim UpdateCQ As New CustomQuery
                UpdateCQ._DB = "Custom"
                Dim InputQuery As String = "Update " & TableName & " SET "
                InputQuery = InputQuery & "  "

                Dim ParameterList1 As New List(Of String)
                Dim CustomQueryParameters As New Dictionary(Of String, String)

                CustomQueryParameters.Add("@DocEntry", DocEntry)

                ParameterList1.Add("SubmittedToSAP=@SubmittedToSAP")
                CustomQueryParameters.Add("@SubmittedToSAP", 0)

                If SyncedToSAP = True Then
                    ParameterList1.Add("SyncedToSAP=@SyncedToSAP")
                    CustomQueryParameters.Add("@SyncedToSAP", 1)
                    ParameterList1.Add("SAPDocNum=@SAPDocNum")
                    CustomQueryParameters.Add("@SAPDocNum", SAPDocNum)
                    ParameterList1.Add("Status=@Status")
                    CustomQueryParameters.Add("@Status", DocumentStatuses.Closed)
                    ParameterList1.Add("SyncStatus=@SyncStatus")
                    CustomQueryParameters.Add("@SyncStatus", SyncStatuses.Synced_Successfully)
                    If Not SAPPaymentDocNum.Equals(String.Empty) Then
                        ParameterList1.Add("SAPPaymentDocNum=@SAPPaymentDocNum")
                        CustomQueryParameters.Add("@SAPPaymentDocNum", SAPPaymentDocNum)
                    End If
                Else
                    ParameterList1.Add("Status=@Status")
                    CustomQueryParameters.Add("@Status", DocumentStatuses.Closed)
                    ParameterList1.Add("SyncedToSAP=@SyncedToSAP")
                    CustomQueryParameters.Add("@SyncedToSAP", 0)
                    ParameterList1.Add("SyncStatus=@SyncStatus")
                    CustomQueryParameters.Add("@SyncStatus", SyncStatuses.Sync_Failed)
                End If




                If Not SyncRemarks.Equals("") Then
                    ParameterList1.Add("SyncRemarks=@SyncRemarks")
                    CustomQueryParameters.Add("@SyncRemarks", SyncRemarks)
                End If
                If ParameterList1.Count > 0 Then
                    Dim CondiString1 As String = String.Join(",", ParameterList1)
                    InputQuery = InputQuery & "" & CondiString1
                End If

                '**********************Query Builder Function *****************
                UpdateCQ._InputQuery = InputQuery & " WHERE DocEntry=@DocEntry "
                UpdateCQ._Parameters = CustomQueryParameters

                ReturnResult = CURD.CustomQueryUpdateData(UpdateCQ)


                Return ReturnResult
            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
                Return False
            End Try
        End Function
        Public Shared Function WriteLog(ByRef ex As Exception) As SyncErrLog
            Dim EL As New SyncErrLog
            Dim St As StackTrace = New StackTrace(ex, True)
            EL.errMSg = ex.Message
            If Not IsNothing(ex.InnerException) Then
                EL.InnerException = ex.InnerException.Message
            End If
            Dim FrameCount As Integer = St.FrameCount
            For i As Integer = 0 To FrameCount - 1
                If Not IsNothing(St.GetFrame(i).GetFileName) Then
                    EL.FileName = St.GetFrame(i).GetFileName.ToString
                    EL.LineNumber = St.GetFrame(i).GetFileLineNumber.ToString
                End If

            Next
            EL.CreatedOn = Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss")
            CURD.InsertData(EL, True)
            Return EL
        End Function
        Public Shared Function WriteSyncLog(ByVal DocType As String, Status As String, ByVal Msg As String) As SyncLog
            Try
                Dim EL As New SyncLog
                EL.Msg = Msg
                EL.DocType = DocType
                EL.Status = Status
                EL.CreatedOn = Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss")
                CURD.InsertData(EL, True)
                Return EL
            Catch ex As Exception
                WriteLog(ex)
                Return New SyncLog
            End Try
        End Function
        Public Shared Sub DataTableToObject(ByRef Obj As Object, ByVal DT As DataTable)
            Try
                Dim props As Type = Obj.GetType
                If DT.Rows.Count > 0 Then
                    Dim Drow As DataRow = DT.Rows(0)
                    For Each member As PropertyInfo In props.GetProperties
                        If Not IsDBNull(Drow(member.Name)) Then
                            member.SetValue(Obj, Drow(member.Name).ToString, Nothing)
                        End If
                    Next
                End If
            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
            End Try
        End Sub
        Public Shared Sub DataRowToObject(ByRef Obj As Object, ByVal DR As DataRow)
            Try
                Dim props As Type = Obj.GetType

                For Each member As PropertyInfo In props.GetProperties
                    If Not IsDBNull(DR(member.Name)) Then
                        member.SetValue(Obj, DR(member.Name).ToString, Nothing)
                    End If
                Next

            Catch ex As Exception
                AppSpecificFunc.WriteLog(ex)
            End Try
        End Sub
    End Class
End Namespace

