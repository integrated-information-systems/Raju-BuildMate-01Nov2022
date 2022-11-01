Imports System.Configuration
Imports BMSS_Sync_Program.Models
Imports System.Text
Imports BMSS.Domain.Abstract
Imports BMSS.Domain.Concrete
Imports BMSS.Domain
Imports BMSS.Domain.Entities
Imports BMSS.Domain.Concrete.SAP
Imports BMSS.Domain.Abstract.SAP

Public Class Form1
    Public CurrentProcess As Processes = Processes.Process_Syncing_AR_Invoice
    Dim StartingTime As DateTime = DateTime.Now
    Private Sub Process_Timer_Tick(sender As Object, e As EventArgs) Handles Process_Timer.Tick
        Try
            Process_Timer.Enabled = False



            Dim TS As TimeSpan = DateTime.Now.Subtract(StartingTime)
            If TS.Minutes > 50 Then
                Application.Exit()
            End If

            ProcessSelector()

            If CurrentProcess = Processes.Process_Syncing_Change_CR_Limit Then
                CurrentProcess = Processes.Process_Syncing_AR_Invoice
            Else
                CurrentProcess = CurrentProcess + 1
            End If


        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Private Sub ProcessSelector()
        Try

            Select Case CurrentProcess
                Case Processes.Process_Syncing_AR_Invoice
                    Func_AR_Invoice()
                Case Processes.Process_Syncing_AR_Invoice_And_Incoming_Payment
                    Func_AR_Invoice_And_Incoming_Payment()
                Case Processes.Process_Syncing_AR_Invoice_And_Outgoing_Payment
                    Func_AR_Invoice_And_Outgoing_Payment()
                Case Processes.Process_Syncing_Goods_Receipt_PO
                    Func_Goods_Receipt_PO()
                Case Processes.Process_Syncing_Goods_Issue
                    Func_Goods_Issue()
                Case Processes.Process_Syncing_Goods_Receipt
                    Func_Goods_Receipt()
                Case Processes.Process_Syncing_Goods_Transfer
                    Func_Goods_Transfer()
                Case Processes.Process_Syncing_Payment
                    Func_Payment()
                Case Processes.Process_Syncing_Change_CR_Limit
                    Func_Change_CR_Limit()
            End Select
            Process_Timer.Enabled = True
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub GetSAPCompanyObject(ByRef SAPCompany As SAPbobsCOM.Company)
        Try

            SAPCompany.Server = ConfigurationManager.AppSettings("Company_Server")
            SAPCompany.LicenseServer = ConfigurationManager.AppSettings("Company_Server")
            'SAPCompany.Server = "192.168.3.118:30000"
            'SAPCompany.LicenseServer = "192.168.3.118"

            SAPCompany.language = SAPbobsCOM.BoSuppLangs.ln_English

            SAPCompany.CompanyDB = ConfigurationManager.AppSettings("Company_DB")
            SAPCompany.UserName = ConfigurationManager.AppSettings("Company_Username")
            SAPCompany.Password = ConfigurationManager.AppSettings("Company_Password")
            SAPCompany.DbUserName = ConfigurationManager.AppSettings("DB_Username")
            SAPCompany.DbPassword = ConfigurationManager.AppSettings("DB_Password")
            SAPCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
            SAPCompany.UseTrusted = False

            '// Connecting to a company DB
            Dim SAPConnectionStatus As Integer
            Dim sErrMsg As String = String.Empty
            Dim sErr As Integer = 0
            If SAPCompany.Connected = False Then
                SAPConnectionStatus = SAPCompany.Connect
                If SAPConnectionStatus <> 0 Then
                    sErrMsg = SAPCompany.GetLastErrorDescription
                    SAPCompany.GetLastError(sErr, sErrMsg)
                    SAPCompany = Nothing
                    Throw New Exception(sErr & "-" & sErrMsg)
                End If
            End If

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub

    Protected Sub Func_Goods_Issue()

        Try
            Dim GIHeaderObj As New StockIssueDocH
            Dim GIItemsObj As StockIssueDocLs() = Nothing
            Dim Lines As IEnumerable(Of StockIssueDocLs) = Nothing


            Using i_StockIssueDocH_Repository As I_StockIssueDocH_Repository = New EF_StockIssueDocHeader_Repository()
                GIHeaderObj = i_StockIssueDocH_Repository.GetDocumentWaitForSyncing()

                If (Not IsNothing(GIHeaderObj)) Then

                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then
                        Dim GIDocument As SAPbobsCOM.Documents
                        GIDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit)
                        GIDocument.UserFields.Fields.Item("U_webref").Value = GIHeaderObj.DocNum
                        GIDocument.Reference2 = GIHeaderObj.DocNum
                        GIDocument.DocDate = GIHeaderObj.DocDate


                        Using i_StockIssueDocLs_Repository As I_StockIssueDocLs_Repository = New EF_StockIssueDocLine_Repository()

                            Lines = i_StockIssueDocLs_Repository.GetSILinesByDocNum(GIHeaderObj.DocNum)
                            GIItemsObj = Lines.ToArray()

                            'GIItemsObj = LoadStockIssueItemsFromDataBase(GIHeaderObj.DocEntry)
                            For i As Integer = 0 To GIItemsObj.Count - 1
                                GIDocument.Lines.ItemCode = GIItemsObj(i).ItemCode
                                If IsNothing(GIItemsObj(i).OverwriteDescription) Then
                                    GIDocument.Lines.ItemDescription = GIItemsObj(i).Description
                                ElseIf GIItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    GIDocument.Lines.ItemDescription = GIItemsObj(i).Description
                                Else
                                    GIDocument.Lines.ItemDescription = GIItemsObj(i).OverwriteDescription
                                End If
                                'GIDocument.Lines.UserFields.Fields.Item("U_OverwriteDescription").Value = InvoiceItemsObj(i).OverwriteDescription
                                If Not IsNothing(GIItemsObj(i).Description2) Then
                                    GIDocument.Lines.UserFields.Fields.Item("U_Description2").Value = GIItemsObj(i).Description2
                                End If
                                If Not IsNothing(GIItemsObj(i).Description3) Then
                                    GIDocument.Lines.UserFields.Fields.Item("U_Description3").Value = GIItemsObj(i).Description3
                                End If
                                If Not IsNothing(GIItemsObj(i).Description4) Then
                                    GIDocument.Lines.UserFields.Fields.Item("U_Description4").Value = GIItemsObj(i).Description4
                                End If
                                If Not IsNothing(GIItemsObj(i).Description5) Then
                                    GIDocument.Lines.UserFields.Fields.Item("U_Description5").Value = GIItemsObj(i).Description5
                                End If
                                If Not IsNothing(GIItemsObj(i).Description6) Then
                                    GIDocument.Lines.UserFields.Fields.Item("U_Description6").Value = GIItemsObj(i).Description6
                                End If
                                GIDocument.Lines.WarehouseCode = GIItemsObj(i).Location
                                GIDocument.Lines.Quantity = GIItemsObj(i).Qty
                                'GRDocument.Lines.UnitPrice = GRItemsObj(i).UnitPrice
                                GIDocument.Lines.Add()
                            Next
                        End Using
                        DocSuccess = GIDocument.Add()
                        Dim DocType As String = "Stock Issue"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OIGE", DocEntry.ToString)

                            SyncMessage = "Goods Issue created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & GIHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockIssueDocH", True, DocNum, GIHeaderObj.DocEntry, SyncMessage)


                            i_StockIssueDocH_Repository.WriteSIInventoryLogs(Lines, GIHeaderObj.DocNum, "Sync Program", "Stock Issue- SAP Sync")
                            i_StockIssueDocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "Stock Issue- SAP Sync")
                            i_StockIssueDocH_Repository.CommitChanges()

                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "Goods Issue creation failed for Stock Issue: " & GIHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockIssueDocH", False, "", GIHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing

                    End If
                End If
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Public Sub WriteSyncLog(ByVal DocType As String, Status As String, ByVal Msg As String)
        Try
            Using i_Log_Repository As I_Log_Repository = New EF_Log_Repository()
                Dim EL As New BMSS.Domain.SyncLog
                EL.Msg = Msg
                EL.DocType = DocType
                EL.Status = Status
                EL.CreatedOn = Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss")
                i_Log_Repository.WriteLog(EL)
            End Using
        Catch ex As Exception
            WriteLog(ex)
        End Try
    End Sub
    Public Sub WriteLog(ByRef ex As Exception)
        Using i_Log_Repository As I_Log_Repository = New EF_Log_Repository()
            Dim EL As New BMSS.Domain.SyncErrLog
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
            i_Log_Repository.WriteSyncLog(EL)
        End Using

    End Sub
    Protected Sub Func_Goods_Transfer()
        Try
            Dim GTHeaderObj As New StockTransDocH
            Dim GTItemsObj As StockTransDocLs() = Nothing
            Dim Lines As IEnumerable(Of StockTransDocLs) = Nothing


            Using i_StockTransDocH_Repository As I_StockTransDocH_Repository = New EF_StockTransDocHeader_Repository()
                GTHeaderObj = i_StockTransDocH_Repository.GetDocumentWaitForSyncing()

                If (Not IsNothing(GTHeaderObj)) Then

                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then
                        Dim GTDocument As SAPbobsCOM.StockTransfer
                        GTDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer)
                        'GTDocument.Reference1 = GRHeaderObj.DocNum
                        GTDocument.UserFields.Fields.Item("U_webref").Value = GTHeaderObj.DocNum
                        GTDocument.Reference2 = GTHeaderObj.DocNum
                        GTDocument.DocDate = GTHeaderObj.DocDate


                        Using i_StockTransDocLs_Repository As I_StockTransDocLs_Repository = New EF_StockTransDocLine_Repository()

                            Lines = i_StockTransDocLs_Repository.GetSTLinesByDocNum(GTHeaderObj.DocNum)
                            GTItemsObj = Lines.ToArray()

                            'GTItemsObj = LoadStockTransferItemsFromDataBase(GTHeaderObj.DocEntry)
                            For i As Integer = 0 To GTItemsObj.Count - 1
                                GTDocument.Lines.ItemCode = GTItemsObj(i).ItemCode
                                If IsNothing(GTItemsObj(i).OverwriteDescription) Then
                                    GTDocument.Lines.ItemDescription = GTItemsObj(i).Description
                                ElseIf GTItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    GTDocument.Lines.ItemDescription = GTItemsObj(i).Description
                                Else
                                    GTDocument.Lines.ItemDescription = GTItemsObj(i).OverwriteDescription
                                End If
                                'GTDocument.Lines.UserFields.Fields.Item("U_OverwriteDescription").Value = GTItemsObj(i).OverwriteDescription
                                If Not IsNothing(GTItemsObj(i).Description2) Then
                                    GTDocument.Lines.UserFields.Fields.Item("U_Description2").Value = GTItemsObj(i).Description2
                                End If
                                If Not IsNothing(GTItemsObj(i).Description3) Then
                                    GTDocument.Lines.UserFields.Fields.Item("U_Description3").Value = GTItemsObj(i).Description3
                                End If
                                If Not IsNothing(GTItemsObj(i).Description4) Then
                                    GTDocument.Lines.UserFields.Fields.Item("U_Description4").Value = GTItemsObj(i).Description4
                                End If
                                If Not IsNothing(GTItemsObj(i).Description5) Then
                                    GTDocument.Lines.UserFields.Fields.Item("U_Description5").Value = GTItemsObj(i).Description5
                                End If
                                If Not IsNothing(GTItemsObj(i).Description6) Then
                                    GTDocument.Lines.UserFields.Fields.Item("U_Description6").Value = GTItemsObj(i).Description6
                                End If
                                GTDocument.Lines.FromWarehouseCode = GTItemsObj(i).FromLocation
                                GTDocument.Lines.Quantity = GTItemsObj(i).Qty
                                GTDocument.Lines.WarehouseCode = GTItemsObj(i).ToLocation

                                GTDocument.Lines.Add()
                            Next

                        End Using
                        DocSuccess = GTDocument.Add()
                        Dim DocType As String = "Stock Transfer"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OWTR", DocEntry.ToString)

                            SyncMessage = "Inventory Transfer created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & GTHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockTransDocH", True, DocNum, GTHeaderObj.DocEntry, SyncMessage)


                            i_StockTransDocH_Repository.WriteSTSourceLocationInventoryLogs(Lines, GTHeaderObj.DocNum, "Sync Program", "Stock Transfer source- SAP Sync")
                            i_StockTransDocH_Repository.WriteSTDestLocationInventoryLogs(Lines, GTHeaderObj.DocNum, "Sync Program", "Stock Transfer destination- SAP Sync")
                            i_StockTransDocH_Repository.UpdateStockBalance(Lines, "Stock Transfer source- SAP Sync")
                            i_StockTransDocH_Repository.CommitChanges()

                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "Inventory Transfer creation failed for Stock Transfer: " & GTHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockTransDocH", False, "", GTHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing

                    End If
                End If
            End Using

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_Goods_Receipt()
        Try
            Dim GRHeaderObj As New StockReceiptDocH
            Dim GRItemsObj As StockReceiptDocLs() = Nothing
            Dim Lines As IEnumerable(Of StockReceiptDocLs) = Nothing

            Using i_StockReceiptDocH_Repository As I_StockReceiptDocH_Repository = New EF_StockReceiptDocHeader_Repository()
                GRHeaderObj = i_StockReceiptDocH_Repository.GetDocumentWaitForSyncing()

                If (Not IsNothing(GRHeaderObj)) Then
                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then
                        Dim GRDocument As SAPbobsCOM.Documents
                        GRDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry)
                        GRDocument.UserFields.Fields.Item("U_webref").Value = GRHeaderObj.DocNum
                        GRDocument.Reference2 = GRHeaderObj.DocNum
                        GRDocument.DocDate = GRHeaderObj.DocDate
                        GRDocument.Comments = GRHeaderObj.Ref
                        Using i_StockReceiptDocLs_Repository As I_StockReceiptDocLs_Repository = New EF_StockReceiptDocLine_Repository()

                            Lines = i_StockReceiptDocLs_Repository.GetSRLinesByDocNum(GRHeaderObj.DocNum)
                            GRItemsObj = Lines.ToArray()

                            'GRItemsObj = LoadStockReceiptItemsFromDataBase(GRHeaderObj.DocEntry)
                            For i As Integer = 0 To GRItemsObj.Count - 1
                                GRDocument.Lines.ItemCode = GRItemsObj(i).ItemCode
                                If IsNothing(GRItemsObj(i).OverwriteDescription) Then
                                    GRDocument.Lines.ItemDescription = GRItemsObj(i).Description
                                ElseIf GRItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    GRDocument.Lines.ItemDescription = GRItemsObj(i).Description
                                Else
                                    GRDocument.Lines.ItemDescription = GRItemsObj(i).OverwriteDescription
                                End If
                                'GRDocument.Lines.UserFields.Fields.Item("U_OverwriteDescription").Value = InvoiceItemsObj(i).OverwriteDescription
                                If Not IsNothing(GRItemsObj(i).Description2) Then
                                    GRDocument.Lines.UserFields.Fields.Item("U_Description2").Value = GRItemsObj(i).Description2
                                End If
                                If Not IsNothing(GRItemsObj(i).Description3) Then
                                    GRDocument.Lines.UserFields.Fields.Item("U_Description3").Value = GRItemsObj(i).Description3
                                End If
                                If Not IsNothing(GRItemsObj(i).Description4) Then
                                    GRDocument.Lines.UserFields.Fields.Item("U_Description4").Value = GRItemsObj(i).Description4
                                End If
                                If Not IsNothing(GRItemsObj(i).Description5) Then
                                    GRDocument.Lines.UserFields.Fields.Item("U_Description5").Value = GRItemsObj(i).Description5
                                End If
                                If Not IsNothing(GRItemsObj(i).Description6) Then
                                    GRDocument.Lines.UserFields.Fields.Item("U_Description6").Value = GRItemsObj(i).Description6
                                End If
                                GRDocument.Lines.WarehouseCode = GRItemsObj(i).Location
                                GRDocument.Lines.Quantity = GRItemsObj(i).Qty
                                GRDocument.Lines.UnitPrice = GRItemsObj(i).UnitPrice



                                GRDocument.Lines.Add()
                            Next
                        End Using
                        DocSuccess = GRDocument.Add()
                        Dim DocType As String = "Stock Receipt"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OIGN", DocEntry.ToString)

                            SyncMessage = "Goods Receipt created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & GRHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockReceiptDocH", True, DocNum, GRHeaderObj.DocEntry, SyncMessage)


                            i_StockReceiptDocH_Repository.WriteSRInventoryLogs(Lines, GRHeaderObj.DocNum, "Sync Program", "Stock Receipt- SAP Sync")
                            i_StockReceiptDocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "Stock Receipt- SAP Sync")
                            i_StockReceiptDocH_Repository.CommitChanges()


                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "Goods Receipt creation failed for Stock Receipt: " & GRHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("StockReceiptDocH", False, "", GRHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing

                    End If
                End If
            End Using

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_Goods_Receipt_PO()
        Try
            Dim GRPOHeaderObj As New GRPODocH
            Dim GRPOItemsObj As GRPODocLs() = Nothing
            Dim Lines As IEnumerable(Of GRPODocLs) = Nothing

            Using i_GRPODocH_Repository As I_GRPODocH_Repository = New EF_GRPODocHeader_Repository()
                GRPOHeaderObj = i_GRPODocH_Repository.GetDocumentWaitForSyncing()
                If (Not IsNothing(GRPOHeaderObj)) Then
                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then

                        Dim GRPODocument As SAPbobsCOM.Documents
                        GRPODocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes)
                        'Header Fields
                        GRPODocument.UserFields.Fields.Item("U_webref").Value = GRPOHeaderObj.DocNum
                        GRPODocument.CardCode = GRPOHeaderObj.CardCode
                        GRPODocument.CardName = GRPOHeaderObj.CardName
                        GRPODocument.NumAtCard = GRPOHeaderObj.DocNum
                        Dim ValidDueDate As Date = GRPOHeaderObj.DueDate
                        GRPODocument.DocDueDate = ValidDueDate.ToString("yyyy-MM-dd")
                        Dim ValidDocDate As Date = GRPOHeaderObj.DocDate
                        GRPODocument.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                        GRPODocument.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")
                        If Not IsNothing(GRPOHeaderObj.DeliveryDate) Then
                            Dim ValidDeliveryDate As Date = GRPOHeaderObj.DeliveryDate
                            GRPODocument.UserFields.Fields.Item("U_DeliveryDate").Value = ValidDeliveryDate.ToString("yyyy-MM-dd")
                        End If

                        GRPODocument.PaymentGroupCode = GRPOHeaderObj.PaymentTerm
                        GRPODocument.SalesPersonCode = GRPOHeaderObj.SlpCode
                        'GRPODocument.UserFields.Fields.Item("U_CustomerTelNo ").Value = GRPOHeaderObj.OfficeTelNo
                        If (GRPOHeaderObj.DiscPercent = 0) Then
                            GRPODocument.DiscountPercent = Math.Round((Decimal.Parse(GRPOHeaderObj.DiscAmount) / Decimal.Parse(GRPOHeaderObj.GrandTotal) * 100), 6, MidpointRounding.AwayFromZero) '10                                                
                        Else
                            GRPODocument.DiscountPercent = GRPOHeaderObj.DiscPercent
                        End If
                        GRPODocument.DocRate = GRPOHeaderObj.ExRate
                        GRPODocument.DocCurrency = GRPOHeaderObj.Currency
                        'GRPODocument.UserFields.Fields.Item("U_HeaderRemarks1").Value = GRPOHeaderObj.HeaderRemarks1
                        'GRPODocument.UserFields.Fields.Item("U_HeaderRemarks2").Value = GRPOHeaderObj.HeaderRemarks2
                        'GRPODocument.UserFields.Fields.Item("U_HeaderRemarks3").Value = GRPOHeaderObj.HeaderRemarks3
                        'GRPODocument.UserFields.Fields.Item("U_HeaderRemarks4").Value = GRPOHeaderObj.HeaderRemarks4
                        'GRPODocument.UserFields.Fields.Item("U_FooterRemarks1").Value = GRPOHeaderObj.FooterRemarks1
                        'GRPODocument.UserFields.Fields.Item("U_FooterRemarks2").Value = GRPOHeaderObj.FooterRemarks2
                        'GRPODocument.UserFields.Fields.Item("U_FooterRemarks3").Value = GRPOHeaderObj.FooterRemarks3
                        'GRPODocument.UserFields.Fields.Item("U_FooterRemarks4").Value = GRPOHeaderObj.FooterRemarks4
                        'GRPODocument.UserFields.Fields.Item("U_SelfCollect").Value = GRPOHeaderObj.SelfCollect
                        GRPODocument.ShipToCode = GRPOHeaderObj.ShipTo
                        GRPODocument.PayToCode = GRPOHeaderObj.BillTo

                        Dim BillingAddress As String = String.Empty
                        Dim ShippingAddress As String = String.Empty

                        If GRPOHeaderObj.BillToAddress1 <> Nothing Then
                            If Not GRPOHeaderObj.BillToAddress1.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & GRPOHeaderObj.BillToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If GRPOHeaderObj.BillToAddress2 <> Nothing Then
                            If Not GRPOHeaderObj.BillToAddress2.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & GRPOHeaderObj.BillToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If GRPOHeaderObj.BillToAddress3 <> Nothing Then
                            If Not GRPOHeaderObj.BillToAddress3.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & GRPOHeaderObj.BillToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If GRPOHeaderObj.BillToAddress4 <> Nothing Then
                            If Not GRPOHeaderObj.BillToAddress4.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & GRPOHeaderObj.BillToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If GRPOHeaderObj.BillToAddress5 <> Nothing Then
                            If Not GRPOHeaderObj.BillToAddress5.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & GRPOHeaderObj.BillToAddress5
                            End If
                        End If

                        If GRPOHeaderObj.SelfCollect.Equals("False") Then
                            If GRPOHeaderObj.ShipToAddress1 <> Nothing Then
                                If Not GRPOHeaderObj.ShipToAddress1.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.ShipToAddress1 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.ShipToAddress2 <> Nothing Then
                                If Not GRPOHeaderObj.ShipToAddress2.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.ShipToAddress2 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.ShipToAddress3 <> Nothing Then
                                If Not GRPOHeaderObj.ShipToAddress3.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.ShipToAddress3 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.ShipToAddress4 <> Nothing Then
                                If Not GRPOHeaderObj.ShipToAddress4.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.ShipToAddress4 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.ShipToAddress5 <> Nothing Then
                                If Not GRPOHeaderObj.ShipToAddress5.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.ShipToAddress5
                                End If
                            End If
                        Else
                            If GRPOHeaderObj.SelfCollectRemarks1 <> Nothing Then
                                If Not GRPOHeaderObj.SelfCollectRemarks1.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.SelfCollectRemarks1 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.SelfCollectRemarks2 <> Nothing Then
                                If Not GRPOHeaderObj.SelfCollectRemarks2.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.SelfCollectRemarks2 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.SelfCollectRemarks3 <> Nothing Then
                                If Not GRPOHeaderObj.SelfCollectRemarks3.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.SelfCollectRemarks3 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.SelfCollectRemarks4 <> Nothing Then
                                If Not GRPOHeaderObj.SelfCollectRemarks4.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.SelfCollectRemarks4 & "," & vbCrLf
                                End If
                            End If
                            If GRPOHeaderObj.SelfCollectRemarks5 <> Nothing Then
                                If Not GRPOHeaderObj.SelfCollectRemarks5.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & GRPOHeaderObj.SelfCollectRemarks5
                                End If
                            End If
                        End If



                        GRPODocument.Address = BillingAddress
                        GRPODocument.Address2 = ShippingAddress
                        Using i_GRPODocLs_Repository As I_GRPODocLs_Repository = New EF_GRPODocLine_Repository()

                            Lines = i_GRPODocLs_Repository.GetGRPOLinesByDocNum(GRPOHeaderObj.DocNum)
                            GRPOItemsObj = Lines.ToArray()
                            'GRPOItemsObj = LoadGRPOItemsFromDataBase(GRPOHeaderObj.DocEntry)
                            For i As Integer = 0 To GRPOItemsObj.Count - 1
                                GRPODocument.Lines.ItemCode = GRPOItemsObj(i).ItemCode
                                If IsNothing(GRPOItemsObj(i).OverwriteDescription) Then
                                    GRPODocument.Lines.ItemDescription = GRPOItemsObj(i).Description
                                ElseIf GRPOItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    GRPODocument.Lines.ItemDescription = GRPOItemsObj(i).Description
                                Else
                                    GRPODocument.Lines.ItemDescription = GRPOItemsObj(i).OverwriteDescription
                                End If
                                'GRPODocument.Lines.UserFields.Fields.Item("U_OverwriteDescription").Value = InvoiceItemsObj(i).OverwriteDescription
                                If Not IsNothing(GRPOItemsObj(i).Description2) Then
                                    GRPODocument.Lines.UserFields.Fields.Item("U_Description2").Value = GRPOItemsObj(i).Description2
                                End If
                                If Not IsNothing(GRPOItemsObj(i).Description3) Then
                                    GRPODocument.Lines.UserFields.Fields.Item("U_Description3").Value = GRPOItemsObj(i).Description3
                                End If
                                If Not IsNothing(GRPOItemsObj(i).Description4) Then
                                    GRPODocument.Lines.UserFields.Fields.Item("U_Description4").Value = GRPOItemsObj(i).Description4
                                End If
                                If Not IsNothing(GRPOItemsObj(i).Description5) Then
                                    GRPODocument.Lines.UserFields.Fields.Item("U_Description5").Value = GRPOItemsObj(i).Description5
                                End If
                                If Not IsNothing(GRPOItemsObj(i).Description6) Then
                                    GRPODocument.Lines.UserFields.Fields.Item("U_Description6").Value = GRPOItemsObj(i).Description6
                                End If

                                GRPODocument.Lines.WarehouseCode = GRPOItemsObj(i).Location
                                GRPODocument.Lines.Quantity = GRPOItemsObj(i).Qty
                                GRPODocument.Lines.UnitPrice = GRPOItemsObj(i).UnitPrice
                                GRPODocument.Lines.VatGroup = GRPOItemsObj(i).GstName
                                'GRPODocument.Lines.VatGroup = "SI"
                                GRPODocument.Lines.Add()
                            Next
                        End Using
                        DocSuccess = GRPODocument.Add()
                        Dim DocType As String = "PDN"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OPDN", DocEntry.ToString)

                            SyncMessage = "PDN created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & GRPOHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("GRPODocH", True, DocNum, GRPOHeaderObj.DocEntry, SyncMessage)


                            i_GRPODocH_Repository.WriteGRPOInventoryLogs(Lines, GRPOHeaderObj.DocNum, "Sync Program", "GRPO- SAP Sync")
                            i_GRPODocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "GRPO- SAP Sync")
                            i_GRPODocH_Repository.CommitChanges()
                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "PDN creation failed for DO: " & GRPOHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("GRPODocH", False, "", GRPOHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing
                    Else

                    End If
                End If
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_AR_Invoice()
        Try
            Dim InvoiceHeaderObj As New DODocH
            Dim InvoiceItemsObj As DODocLs() = Nothing
            Dim Lines As IEnumerable(Of DODocLs) = Nothing
            Using i_DODocH_Repository As I_DODocH_Repository = New EF_DODocHeader_Repository()
                InvoiceHeaderObj = i_DODocH_Repository.GetDocumentWaitForSyncing()
                If (Not IsNothing(InvoiceHeaderObj)) Then

                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then

                        Dim ARInvoiceDocument As SAPbobsCOM.Documents
                        ARInvoiceDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
                        'Header Fields
                        ARInvoiceDocument.UserFields.Fields.Item("U_webref").Value = InvoiceHeaderObj.DocNum
                        ARInvoiceDocument.CardCode = InvoiceHeaderObj.CardCode
                        ARInvoiceDocument.CardName = InvoiceHeaderObj.CardName
                        ARInvoiceDocument.NumAtCard = InvoiceHeaderObj.DocNum
                        Dim ValidDueDate As Date = InvoiceHeaderObj.DueDate
                        ARInvoiceDocument.DocDueDate = ValidDueDate.ToString("yyyy-MM-dd")
                        Dim ValidDocDate As Date = InvoiceHeaderObj.DocDate
                        ARInvoiceDocument.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                        ARInvoiceDocument.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")
                        If Not IsNothing(InvoiceHeaderObj.DeliveryDate) Then
                            Dim ValidDeliveryDate As Date = InvoiceHeaderObj.DeliveryDate

                            ARInvoiceDocument.UserFields.Fields.Item("U_DeliveryDate").Value = ValidDeliveryDate.ToString("yyyy-MM-dd")
                        End If

                        If (Not IsNothing(InvoiceHeaderObj.DeliveryTime)) Then
                            ARInvoiceDocument.UserFields.Fields.Item("U_DeliveryTime").Value = InvoiceHeaderObj.DeliveryTime
                        End If
                        If (Not IsNothing(InvoiceHeaderObj.CustomerRef)) Then
                            ARInvoiceDocument.UserFields.Fields.Item("U_Custref").Value = InvoiceHeaderObj.CustomerRef
                        End If

                        Dim HeaderRemarks As StringBuilder = New StringBuilder()

                        HeaderRemarks.Append(InvoiceHeaderObj.HeaderRemarks1)
                        HeaderRemarks.Append(" ")
                        HeaderRemarks.Append(InvoiceHeaderObj.HeaderRemarks2)
                        HeaderRemarks.Append(" ")
                        HeaderRemarks.Append(InvoiceHeaderObj.HeaderRemarks3)
                        HeaderRemarks.Append(" ")
                        HeaderRemarks.Append(InvoiceHeaderObj.HeaderRemarks4)
                        ARInvoiceDocument.UserFields.Fields.Item("U_Header").Value = HeaderRemarks.ToString()

                        Dim FooterRemarks As StringBuilder = New StringBuilder()
                        FooterRemarks.Append(InvoiceHeaderObj.FooterRemarks1)
                        FooterRemarks.Append(" ")
                        FooterRemarks.Append(InvoiceHeaderObj.FooterRemarks2)
                        FooterRemarks.Append(" ")
                        FooterRemarks.Append(InvoiceHeaderObj.FooterRemarks3)
                        FooterRemarks.Append(" ")
                        FooterRemarks.Append(InvoiceHeaderObj.FooterRemarks4)
                        ARInvoiceDocument.UserFields.Fields.Item("U_Footer").Value = FooterRemarks.ToString()



                        ARInvoiceDocument.GroupNumber = InvoiceHeaderObj.PaymentTerm
                        ARInvoiceDocument.SalesPersonCode = InvoiceHeaderObj.SlpCode
                        'ARInvoiceDocument.UserFields.Fields.Item("U_CustomerTelNo ").Value = InvoiceHeaderObj.OfficeTelNo
                        If (InvoiceHeaderObj.DiscPercent = 0) Then
                            ARInvoiceDocument.DiscountPercent = Math.Round((Decimal.Parse(InvoiceHeaderObj.DiscAmount) / Decimal.Parse(InvoiceHeaderObj.GrandTotal) * 100), 6, MidpointRounding.AwayFromZero) '10                                                
                        Else
                            ARInvoiceDocument.DiscountPercent = InvoiceHeaderObj.DiscPercent
                        End If

                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks1").Value = InvoiceHeaderObj.HeaderRemarks1
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks2").Value = InvoiceHeaderObj.HeaderRemarks2
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks3").Value = InvoiceHeaderObj.HeaderRemarks3
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks4").Value = InvoiceHeaderObj.HeaderRemarks4
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks1").Value = InvoiceHeaderObj.FooterRemarks1
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks2").Value = InvoiceHeaderObj.FooterRemarks2
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks3").Value = InvoiceHeaderObj.FooterRemarks3
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks4").Value = InvoiceHeaderObj.FooterRemarks4
                        'ARInvoiceDocument.UserFields.Fields.Item("U_SelfCollect").Value = InvoiceHeaderObj.SelfCollect
                        ARInvoiceDocument.ShipToCode = InvoiceHeaderObj.ShipTo
                        ARInvoiceDocument.PayToCode = InvoiceHeaderObj.BillTo

                        Dim BillingAddress As String = String.Empty
                        Dim ShippingAddress As String = String.Empty

                        If InvoiceHeaderObj.BillToAddress1 <> Nothing Then
                            If Not InvoiceHeaderObj.BillToAddress1.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & InvoiceHeaderObj.BillToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If InvoiceHeaderObj.BillToAddress2 <> Nothing Then
                            If Not InvoiceHeaderObj.BillToAddress2.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & InvoiceHeaderObj.BillToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If InvoiceHeaderObj.BillToAddress3 <> Nothing Then
                            If Not InvoiceHeaderObj.BillToAddress3.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & InvoiceHeaderObj.BillToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If InvoiceHeaderObj.BillToAddress4 <> Nothing Then
                            If Not InvoiceHeaderObj.BillToAddress4.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & InvoiceHeaderObj.BillToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If InvoiceHeaderObj.BillToAddress5 <> Nothing Then
                            If Not InvoiceHeaderObj.BillToAddress5.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & InvoiceHeaderObj.BillToAddress5
                            End If
                        End If

                        If InvoiceHeaderObj.SelfCollect.Equals("False") Then
                            If InvoiceHeaderObj.ShipToAddress1 <> Nothing Then
                                If Not InvoiceHeaderObj.ShipToAddress1.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.ShipToAddress1 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.ShipToAddress2 <> Nothing Then
                                If Not InvoiceHeaderObj.ShipToAddress2.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.ShipToAddress2 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.ShipToAddress3 <> Nothing Then
                                If Not InvoiceHeaderObj.ShipToAddress3.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.ShipToAddress3 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.ShipToAddress4 <> Nothing Then
                                If Not InvoiceHeaderObj.ShipToAddress4.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.ShipToAddress4 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.ShipToAddress5 <> Nothing Then
                                If Not InvoiceHeaderObj.ShipToAddress5.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.ShipToAddress5
                                End If
                            End If
                        Else
                            If InvoiceHeaderObj.SelfCollectRemarks1 <> Nothing Then
                                If Not InvoiceHeaderObj.SelfCollectRemarks1.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.SelfCollectRemarks1 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.SelfCollectRemarks2 <> Nothing Then
                                If Not InvoiceHeaderObj.SelfCollectRemarks2.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.SelfCollectRemarks2 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.SelfCollectRemarks3 <> Nothing Then
                                If Not InvoiceHeaderObj.SelfCollectRemarks3.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.SelfCollectRemarks3 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.SelfCollectRemarks4 <> Nothing Then
                                If Not InvoiceHeaderObj.SelfCollectRemarks4.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.SelfCollectRemarks4 & "," & vbCrLf
                                End If
                            End If
                            If InvoiceHeaderObj.SelfCollectRemarks5 <> Nothing Then
                                If Not InvoiceHeaderObj.SelfCollectRemarks5.Trim.Equals(String.Empty) Then
                                    ShippingAddress = ShippingAddress & InvoiceHeaderObj.SelfCollectRemarks5
                                End If
                            End If
                        End If



                        ARInvoiceDocument.Address = BillingAddress
                        ARInvoiceDocument.Address2 = ShippingAddress

                        Using i_DODocLs_Repository As I_DODocLs_Repository = New EF_DODocLine_Repository()

                            Lines = i_DODocLs_Repository.GetDOLinesByDocNum(InvoiceHeaderObj.DocNum)
                            InvoiceItemsObj = Lines.ToArray()
                            For i As Integer = 0 To InvoiceItemsObj.Count - 1
                                ARInvoiceDocument.Lines.ItemCode = InvoiceItemsObj(i).ItemCode
                                If IsNothing(InvoiceItemsObj(i).OverwriteDescription) Then
                                    ARInvoiceDocument.Lines.ItemDescription = InvoiceItemsObj(i).Description
                                ElseIf InvoiceItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    ARInvoiceDocument.Lines.ItemDescription = InvoiceItemsObj(i).Description
                                Else
                                    ARInvoiceDocument.Lines.ItemDescription = InvoiceItemsObj(i).OverwriteDescription
                                End If
                                ARInvoiceDocument.Lines.UserFields.Fields.Item("U_cost").Value = InvoiceItemsObj(i).UnitCost.ToString()
                                If Not IsNothing(InvoiceItemsObj(i).Description2) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description2").Value = InvoiceItemsObj(i).Description2
                                End If
                                If Not IsNothing(InvoiceItemsObj(i).Description3) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description3").Value = InvoiceItemsObj(i).Description3
                                End If
                                If Not IsNothing(InvoiceItemsObj(i).Description4) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description4").Value = InvoiceItemsObj(i).Description4
                                End If
                                If Not IsNothing(InvoiceItemsObj(i).Description5) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description5").Value = InvoiceItemsObj(i).Description5
                                End If
                                If Not IsNothing(InvoiceItemsObj(i).Description6) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description6").Value = InvoiceItemsObj(i).Description6
                                End If
                                ARInvoiceDocument.Lines.WarehouseCode = InvoiceItemsObj(i).Location

                                ARInvoiceDocument.Lines.Quantity = InvoiceItemsObj(i).Qty
                                ARInvoiceDocument.Lines.UnitPrice = InvoiceItemsObj(i).UnitPrice

                                ARInvoiceDocument.Lines.VatGroup = InvoiceItemsObj(i).GstName
                                ARInvoiceDocument.Lines.Add()
                            Next

                        End Using
                        DocSuccess = ARInvoiceDocument.Add()
                        Dim DocType As String = "AR Invoice"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OINV", DocEntry.ToString)

                            SyncMessage = "AR Invoice created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & InvoiceHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("DODocH", True, DocNum, InvoiceHeaderObj.DocEntry, SyncMessage)
                            i_DODocH_Repository.WriteDOInventoryLogs(Lines, InvoiceHeaderObj.DocNum, "Sync Program", "DO- SAP Sync")
                            i_DODocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "DO- SAP Sync")
                            i_DODocH_Repository.CommitChanges()
                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "AR Invoice creation failed for DO: " & InvoiceHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("DODocH", False, "", InvoiceHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing
                    Else

                    End If
                End If
            End Using

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub

    Protected Sub Func_AR_Invoice_And_Outgoing_Payment()
        Try
            Dim CashSalesCreditHeaderObj As New CashSalesCreditDocH
            Dim CashSalesItemsObj As CashSalesCreditDocLs() = Nothing
            Dim CashSalesPaymentsObj As CashSalesCreditDocPays() = Nothing
            Dim Lines As IEnumerable(Of CashSalesCreditDocLs) = Nothing
            Dim PayLines As IEnumerable(Of CashSalesCreditDocPays) = Nothing
            Using i_CashSalesCreditDocH_Repository As I_CashSalesCreditDocH_Repository = New EF_CashSalesCreditDocHeader_Repository()
                CashSalesCreditHeaderObj = i_CashSalesCreditDocH_Repository.GetDocumentWaitForSyncing()
                If (Not IsNothing(CashSalesCreditHeaderObj)) Then
                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then

                        Dim ARCreditNoteDocument As SAPbobsCOM.Documents
                        ARCreditNoteDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes)
                        'Header Fields
                        ARCreditNoteDocument.UserFields.Fields.Item("U_webref").Value = CashSalesCreditHeaderObj.DocNum
                        ARCreditNoteDocument.CardCode = CashSalesCreditHeaderObj.CardCode
                        ARCreditNoteDocument.CardName = CashSalesCreditHeaderObj.CashSalesCardName
                        ARCreditNoteDocument.NumAtCard = CashSalesCreditHeaderObj.DocNum
                        'Dim ValidDueDate As Date = CashSalesCreditHeaderObj.DueDate
                        Dim ValidDocDate As Date = CashSalesCreditHeaderObj.DocDate
                        ARCreditNoteDocument.DocDueDate = ValidDocDate.ToString("yyyy-MM-dd")

                        ARCreditNoteDocument.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                        ARCreditNoteDocument.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")
                        'Dim ValidDeliveryDate As Date = CashSalesCreditHeaderObj.DeliveryDate
                        ARCreditNoteDocument.UserFields.Fields.Item("U_DeliveryDate").Value = ValidDocDate.ToString("yyyy-MM-dd")
                        ARCreditNoteDocument.PaymentGroupCode = CashSalesCreditHeaderObj.PaymentTerm
                        ARCreditNoteDocument.SalesPersonCode = CashSalesCreditHeaderObj.SlpCode
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_CustomerTelNo ").Value = CashSalesCreditHeaderObj.OfficeTelNo
                        If (CashSalesCreditHeaderObj.DiscPercent = 0) Then
                            ARCreditNoteDocument.DiscountPercent = Math.Round((Decimal.Parse(CashSalesCreditHeaderObj.DiscAmount) / Decimal.Parse(CashSalesCreditHeaderObj.GrandTotal) * 100), 6, MidpointRounding.AwayFromZero) '10                                                
                        Else
                            ARCreditNoteDocument.DiscountPercent = CashSalesCreditHeaderObj.DiscPercent
                        End If
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_HeaderRemarks1").Value = CashSalesCreditHeaderObj.HeaderRemarks1
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_HeaderRemarks2").Value = CashSalesCreditHeaderObj.HeaderRemarks2
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_HeaderRemarks3").Value = CashSalesCreditHeaderObj.HeaderRemarks3
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_HeaderRemarks4").Value = CashSalesCreditHeaderObj.HeaderRemarks4
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_FooterRemarks1").Value = CashSalesCreditHeaderObj.FooterRemarks1
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_FooterRemarks2").Value = CashSalesCreditHeaderObj.FooterRemarks2
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_FooterRemarks3").Value = CashSalesCreditHeaderObj.FooterRemarks3
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_FooterRemarks4").Value = CashSalesCreditHeaderObj.FooterRemarks4
                        'ARCreditNoteDocument.UserFields.Fields.Item("U_SelfCollect").Value = CashSalesCreditHeaderObj.SelfCollect
                        'ARCreditNoteDocument.ShipToCode = CashSalesCreditHeaderObj.ShipTo
                        'ARCreditNoteDocument.PayToCode = CashSalesCreditHeaderObj.BillTo

                        Dim BillingAddress As String = String.Empty
                        Dim ShippingAddress As String = String.Empty

                        If CashSalesCreditHeaderObj.BillToAddress1 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.BillToAddress1.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesCreditHeaderObj.BillToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.BillToAddress2 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.BillToAddress2.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesCreditHeaderObj.BillToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.BillToAddress3 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.BillToAddress3.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesCreditHeaderObj.BillToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.BillToAddress4 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.BillToAddress4.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesCreditHeaderObj.BillToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.BillToAddress5 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.BillToAddress5.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesCreditHeaderObj.BillToAddress5
                            End If
                        End If

                        'If CashSalesCreditHeaderObj.SelfCollect.Equals("False") Then
                        If CashSalesCreditHeaderObj.ShipToAddress1 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.ShipToAddress1.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.ShipToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.ShipToAddress2 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.ShipToAddress2.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.ShipToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.ShipToAddress3 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.ShipToAddress3.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.ShipToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.ShipToAddress4 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.ShipToAddress4.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.ShipToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesCreditHeaderObj.ShipToAddress5 <> Nothing Then
                            If Not CashSalesCreditHeaderObj.ShipToAddress5.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.ShipToAddress5
                            End If
                        End If
                        'Else
                        '    If CashSalesCreditHeaderObj.SelfCollectRemarks1 <> Nothing Then
                        '        If Not CashSalesCreditHeaderObj.SelfCollectRemarks1.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.SelfCollectRemarks1 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesCreditHeaderObj.SelfCollectRemarks2 <> Nothing Then
                        '        If Not CashSalesCreditHeaderObj.SelfCollectRemarks2.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.SelfCollectRemarks2 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesCreditHeaderObj.SelfCollectRemarks3 <> Nothing Then
                        '        If Not CashSalesCreditHeaderObj.SelfCollectRemarks3.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.SelfCollectRemarks3 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesCreditHeaderObj.SelfCollectRemarks4 <> Nothing Then
                        '        If Not CashSalesCreditHeaderObj.SelfCollectRemarks4.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.SelfCollectRemarks4 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesCreditHeaderObj.SelfCollectRemarks5 <> Nothing Then
                        '        If Not CashSalesCreditHeaderObj.SelfCollectRemarks5.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesCreditHeaderObj.SelfCollectRemarks5
                        '        End If
                        '    End If
                        'End If



                        ARCreditNoteDocument.Address = BillingAddress
                        ARCreditNoteDocument.Address2 = ShippingAddress

                        Using i_CashSalesCreditDocLs_Repository As I_CashSalesCreditDocLs_Repository = New EF_CashSalesCreditDocLine_Repository()

                            Lines = i_CashSalesCreditDocLs_Repository.GetCSCLinesByDocNum(CashSalesCreditHeaderObj.DocNum)
                            CashSalesItemsObj = Lines.ToArray()
                            PayLines = i_CashSalesCreditDocLs_Repository.GetCSCPayLinesByDocNum(CashSalesCreditHeaderObj.DocNum)
                            CashSalesPaymentsObj = PayLines.ToArray()

                            '    CashSalesItemsObj = LoadCashSalesCreditItemsFromDataBase(CashSalesCreditHeaderObj.DocEntry)
                            'CashSalesPaymentsObj = LoadCashSalesCreditPaymentsFromDataBase(CashSalesCreditHeaderObj.DocEntry
                            For i As Integer = 0 To CashSalesItemsObj.Count - 1
                                ARCreditNoteDocument.Lines.ItemCode = CashSalesItemsObj(i).ItemCode
                                If IsNothing(CashSalesItemsObj(i).OverwriteDescription) Then
                                    ARCreditNoteDocument.Lines.ItemDescription = CashSalesItemsObj(i).Description
                                ElseIf CashSalesItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    ARCreditNoteDocument.Lines.ItemDescription = CashSalesItemsObj(i).Description
                                Else
                                    ARCreditNoteDocument.Lines.ItemDescription = CashSalesItemsObj(i).OverwriteDescription
                                End If
                                ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_cost").Value = CashSalesItemsObj(i).UnitCost.ToString()
                                If Not IsNothing(CashSalesItemsObj(i).Description2) Then
                                    ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_Description2").Value = CashSalesItemsObj(i).Description2
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description3) Then
                                    ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_Description3").Value = CashSalesItemsObj(i).Description3
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description4) Then
                                    ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_Description4").Value = CashSalesItemsObj(i).Description4
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description5) Then
                                    ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_Description5").Value = CashSalesItemsObj(i).Description5
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description6) Then
                                    ARCreditNoteDocument.Lines.UserFields.Fields.Item("U_Description6").Value = CashSalesItemsObj(i).Description6
                                End If
                                ARCreditNoteDocument.Lines.WarehouseCode = CashSalesItemsObj(i).Location
                                ARCreditNoteDocument.Lines.Quantity = CashSalesItemsObj(i).Qty
                                ARCreditNoteDocument.Lines.UnitPrice = CashSalesItemsObj(i).UnitPrice
                                ARCreditNoteDocument.Lines.VatGroup = CashSalesItemsObj(i).GstName
                                'ARCreditNoteDocument.Lines.AccountCode = "1231010"
                                ARCreditNoteDocument.Lines.Add()
                            Next
                        End Using
                        DocSuccess = ARCreditNoteDocument.Add()
                        Dim DocType As String = "AR Credit Note"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("ORIN", DocEntry.ToString)

                            SyncMessage = "AR Credit Note created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & CashSalesCreditHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            'AppSpecificFunc.UpdateSyncStaus("CashSalesCreditDocH", True, DocNum, CashSalesCreditHeaderObj.DocEntry, SyncMessage)

                            Dim PaymentErrCode As Integer = 0
                            Dim PaymentErrMsg As String = String.Empty
                            Dim PaymentDocSuccess As Long
                            Dim PaymentDocNum As String = String.Empty

                            Dim oPayment As SAPbobsCOM.Payments
                            oPayment = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments)
                            oPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments
                            oPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer
                            oPayment.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.DueDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.UserFields.Fields.Item("U_webref").Value = CashSalesCreditHeaderObj.DocNum

                            oPayment.CardCode = CashSalesCreditHeaderObj.CardCode
                            oPayment.CardName = CashSalesCreditHeaderObj.CardName
                            oPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_CredItnote
                            oPayment.Invoices.DocEntry = DocEntry
                            'Credit Card
                            For i As Integer = 0 To CashSalesPaymentsObj.Count - 1
                                oPayment.CreditCards.CreditCard = CashSalesPaymentsObj(i).PaymentType
                                oPayment.CreditCards.CreditAcct = CashSalesPaymentsObj(i).GLCode
                                oPayment.CreditCards.FirstPaymentSum = CashSalesPaymentsObj(i).PaidAmount
                                oPayment.CreditCards.CreditSum = CashSalesPaymentsObj(i).PaidAmount
                                oPayment.CreditCards.CreditCardNumber = "1111 1111 1111 1111"
                                oPayment.CreditCards.CardValidUntil = Date.Now.AddDays(10).ToString("yyyy-MM-dd")
                                oPayment.CreditCards.VoucherNum = "888"
                                'oPayment.CreditCards.UserFields.Fields.Item(" U_ChequeNoReference").Value = CashSalesPaymentsObj(i).ChequeNoReference
                                'oPayment.CreditCards.UserFields.Fields.Item(" U_PaymentRemarks").Value = CashSalesPaymentsObj(i).PaymentRemarks
                                oPayment.CreditCards.Add()
                            Next

                            ' Bank Transfer
                            'oPayment.TransferSum = OrderHeaderObj.OrderTotal
                            'oPayment.TransferAccount = 161016 ' My PC Local Account Code for Testing
                            'oPayment.TransferAccount = 121010

                            ' Cash
                            'oPayment.CashSum = OrderHeaderObj.OrderTotal
                            'oPayment.CashAccount = 161016

                            PaymentDocSuccess = oPayment.Add()
                            If PaymentDocSuccess = 0 Then
                                Dim PaymentDocEntry As Integer = 0
                                PaymentDocEntry = SAPCompany.GetNewObjectKey()
                                PaymentDocNum = CURD.GetDocNum("OVPM", PaymentDocEntry.ToString)
                                SyncMessage = SyncMessage & ", AR Credit Note Payment created SAP Doc No: " & PaymentDocNum & " Based on BMSS DocNum: " & CashSalesCreditHeaderObj.DocNum
                                WriteSyncLog("Payment", "Succeeded", SyncMessage)
                                AppSpecificFunc.UpdateSyncStaus("CashSalesCreditDocH", True, DocNum, CashSalesCreditHeaderObj.DocEntry, SyncMessage, PaymentDocNum)

                                i_CashSalesCreditDocH_Repository.WriteCSCInventoryLogs(Lines, CashSalesCreditHeaderObj.DocNum, "Sync Program", "CSC - SAP Sync")
                                i_CashSalesCreditDocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "CSC - SAP Sync")
                                i_CashSalesCreditDocH_Repository.CommitChanges()
                            Else
                                Dim ErrCode As Integer = 0
                                Dim ErrMsg As String = String.Empty
                                SAPCompany.GetLastError(ErrCode, ErrMsg)
                                Dim SyncErrMessage As String = String.Empty
                                SyncErrMessage = "Payment creation failed for Cash Sales: " & CashSalesCreditHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                                WriteSyncLog(DocType, "Failed", SyncErrMessage)
                                AppSpecificFunc.UpdateSyncStaus("CashSalesCreditDocH", False, "", CashSalesCreditHeaderObj.DocEntry, SyncErrMessage)
                            End If
                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "AR Credit Note creation failed for DO: " & CashSalesCreditHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("CashSalesCreditDocH", False, "", CashSalesCreditHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing
                    Else

                    End If
                End If
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_AR_Invoice_And_Incoming_Payment()
        Try
            Dim CashSalesHeaderObj As New CashSalesDocH
            Dim CashSalesItemsObj As CashSalesDocLs() = Nothing
            Dim CashSalesPaymentsObj As CashSalesDocPays() = Nothing
            Dim Lines As IEnumerable(Of CashSalesDocLs) = Nothing
            Dim PayLines As IEnumerable(Of CashSalesDocPays) = Nothing
            Dim CreditCards As IEnumerable(Of OCRC) = Nothing

            Using i_OCRC_Repository As I_OCRC_Repository = New EF_OCRC_Repository()
                CreditCards = i_OCRC_Repository.CreditCards
            End Using

            Using i_CashSalesDocH_Repository As I_CashSalesDocH_Repository = New EF_CashSalesDocHeader_Repository()
                CashSalesHeaderObj = i_CashSalesDocH_Repository.GetDocumentWaitForSyncing()
                If (Not IsNothing(CashSalesHeaderObj)) Then
                    Dim SAPCompany As New SAPbobsCOM.Company
                    Dim DocSuccess As Long
                    GetSAPCompanyObject(SAPCompany)
                    If Not IsNothing(SAPCompany) Then

                        Dim ARInvoiceDocument As SAPbobsCOM.Documents
                        ARInvoiceDocument = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
                        'Header Fields
                        ARInvoiceDocument.UserFields.Fields.Item("U_webref").Value = CashSalesHeaderObj.DocNum
                        ARInvoiceDocument.CardCode = CashSalesHeaderObj.CardCode
                        ARInvoiceDocument.CardName = CashSalesHeaderObj.CashSalesCardName
                        ARInvoiceDocument.NumAtCard = CashSalesHeaderObj.DocNum
                        'Dim ValidDueDate As Date = CashSalesHeaderObj.DueDate
                        Dim ValidDocDate As Date = CashSalesHeaderObj.DocDate
                        ARInvoiceDocument.DocDueDate = ValidDocDate.ToString("yyyy-MM-dd")
                        ARInvoiceDocument.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                        ARInvoiceDocument.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")

                        If Not IsNothing(CashSalesHeaderObj.DeliveryDate) Then
                            Dim ValidDeliveryDate As Date = CashSalesHeaderObj.DeliveryDate
                            ARInvoiceDocument.UserFields.Fields.Item("U_DeliveryDate").Value = ValidDeliveryDate.ToString("yyyy-MM-dd")
                        End If

                        ARInvoiceDocument.PaymentGroupCode = CashSalesHeaderObj.PaymentTerm
                        ARInvoiceDocument.SalesPersonCode = CashSalesHeaderObj.SlpCode
                        'ARInvoiceDocument.UserFields.Fields.Item("U_CustomerTelNo ").Value = CashSalesHeaderObj.OfficeTelNo
                        If (CashSalesHeaderObj.DiscPercent = 0) Then
                            ARInvoiceDocument.DiscountPercent = Math.Round((Decimal.Parse(CashSalesHeaderObj.DiscAmount) / Decimal.Parse(CashSalesHeaderObj.GrandTotal) * 100), 6, MidpointRounding.AwayFromZero) '10                                                
                        Else
                            ARInvoiceDocument.DiscountPercent = CashSalesHeaderObj.DiscPercent
                        End If
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks1").Value = CashSalesHeaderObj.HeaderRemarks1
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks2").Value = CashSalesHeaderObj.HeaderRemarks2
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks3").Value = CashSalesHeaderObj.HeaderRemarks3
                        'ARInvoiceDocument.UserFields.Fields.Item("U_HeaderRemarks4").Value = CashSalesHeaderObj.HeaderRemarks4
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks1").Value = CashSalesHeaderObj.FooterRemarks1
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks2").Value = CashSalesHeaderObj.FooterRemarks2
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks3").Value = CashSalesHeaderObj.FooterRemarks3
                        'ARInvoiceDocument.UserFields.Fields.Item("U_FooterRemarks4").Value = CashSalesHeaderObj.FooterRemarks4
                        'ARInvoiceDocument.UserFields.Fields.Item("U_SelfCollect").Value = CashSalesHeaderObj.SelfCollect
                        'ARInvoiceDocument.ShipToCode = CashSalesHeaderObj.ShipTo
                        'ARInvoiceDocument.PayToCode = CashSalesHeaderObj.BillTo

                        Dim BillingAddress As String = String.Empty
                        Dim ShippingAddress As String = String.Empty

                        If CashSalesHeaderObj.BillToAddress1 <> Nothing Then
                            If Not CashSalesHeaderObj.BillToAddress1.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesHeaderObj.BillToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.BillToAddress2 <> Nothing Then
                            If Not CashSalesHeaderObj.BillToAddress2.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesHeaderObj.BillToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.BillToAddress3 <> Nothing Then
                            If Not CashSalesHeaderObj.BillToAddress3.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesHeaderObj.BillToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.BillToAddress4 <> Nothing Then
                            If Not CashSalesHeaderObj.BillToAddress4.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesHeaderObj.BillToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.BillToAddress5 <> Nothing Then
                            If Not CashSalesHeaderObj.BillToAddress5.Trim.Equals(String.Empty) Then
                                BillingAddress = BillingAddress & CashSalesHeaderObj.BillToAddress5
                            End If
                        End If

                        'If CashSalesHeaderObj.SelfCollect.Equals("False") Then
                        If CashSalesHeaderObj.ShipToAddress1 <> Nothing Then
                            If Not CashSalesHeaderObj.ShipToAddress1.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesHeaderObj.ShipToAddress1 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.ShipToAddress2 <> Nothing Then
                            If Not CashSalesHeaderObj.ShipToAddress2.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesHeaderObj.ShipToAddress2 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.ShipToAddress3 <> Nothing Then
                            If Not CashSalesHeaderObj.ShipToAddress3.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesHeaderObj.ShipToAddress3 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.ShipToAddress4 <> Nothing Then
                            If Not CashSalesHeaderObj.ShipToAddress4.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesHeaderObj.ShipToAddress4 & "," & vbCrLf
                            End If
                        End If
                        If CashSalesHeaderObj.ShipToAddress5 <> Nothing Then
                            If Not CashSalesHeaderObj.ShipToAddress5.Trim.Equals(String.Empty) Then
                                ShippingAddress = ShippingAddress & CashSalesHeaderObj.ShipToAddress5
                            End If
                        End If
                        'Else
                        '    If CashSalesHeaderObj.SelfCollectRemarks1 <> Nothing Then
                        '        If Not CashSalesHeaderObj.SelfCollectRemarks1.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesHeaderObj.SelfCollectRemarks1 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesHeaderObj.SelfCollectRemarks2 <> Nothing Then
                        '        If Not CashSalesHeaderObj.SelfCollectRemarks2.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesHeaderObj.SelfCollectRemarks2 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesHeaderObj.SelfCollectRemarks3 <> Nothing Then
                        '        If Not CashSalesHeaderObj.SelfCollectRemarks3.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesHeaderObj.SelfCollectRemarks3 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesHeaderObj.SelfCollectRemarks4 <> Nothing Then
                        '        If Not CashSalesHeaderObj.SelfCollectRemarks4.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesHeaderObj.SelfCollectRemarks4 & "," & vbCrLf
                        '        End If
                        '    End If
                        '    If CashSalesHeaderObj.SelfCollectRemarks5 <> Nothing Then
                        '        If Not CashSalesHeaderObj.SelfCollectRemarks5.Trim.Equals(String.Empty) Then
                        '            ShippingAddress = ShippingAddress & CashSalesHeaderObj.SelfCollectRemarks5
                        '        End If
                        '    End If
                        'End If



                        ARInvoiceDocument.Address = BillingAddress
                        ARInvoiceDocument.Address2 = ShippingAddress
                        Dim JournalRemarks As String = String.Empty

                        Using i_CashSalesDocLs_Repository As I_CashSalesDocLs_Repository = New EF_CashSalesDocLine_Repository()

                            Lines = i_CashSalesDocLs_Repository.GetCSLinesByDocNum(CashSalesHeaderObj.DocNum)
                            CashSalesItemsObj = Lines.ToArray()
                            PayLines = i_CashSalesDocLs_Repository.GetCSPayLinesByDocNum(CashSalesHeaderObj.DocNum)
                            CashSalesPaymentsObj = PayLines.ToArray()

                            Dim PaymentTypeNames = CreditCards.Join(PayLines, Function(a) a.CreditCard.ToString, Function(b) b.PaymentType, Function(a, b) New With {a.CardName}).ToList()
                            JournalRemarks = String.Join(", ", PaymentTypeNames.Select(Function(x) x.CardName).ToList())




                            'CashSalesItemsObj = LoadCashSalesItemsFromDataBase(CashSalesHeaderObj.DocEntry)
                            'CashSalesPaymentsObj = LoadCashSalesPaymentsFromDataBase(CashSalesHeaderObj.DocEntry)
                            For i As Integer = 0 To CashSalesItemsObj.Count - 1
                                ARInvoiceDocument.Lines.ItemCode = CashSalesItemsObj(i).ItemCode
                                If IsNothing(CashSalesItemsObj(i).OverwriteDescription) Then
                                    ARInvoiceDocument.Lines.ItemDescription = CashSalesItemsObj(i).Description
                                ElseIf CashSalesItemsObj(i).OverwriteDescription.Trim.ToString.Equals("") Then
                                    ARInvoiceDocument.Lines.ItemDescription = CashSalesItemsObj(i).Description
                                Else
                                    ARInvoiceDocument.Lines.ItemDescription = CashSalesItemsObj(i).OverwriteDescription
                                End If
                                ARInvoiceDocument.Lines.UserFields.Fields.Item("U_cost").Value = CashSalesItemsObj(i).UnitCost.ToString()
                                If Not IsNothing(CashSalesItemsObj(i).Description2) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description2").Value = CashSalesItemsObj(i).Description2
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description3) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description3").Value = CashSalesItemsObj(i).Description3
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description4) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description4").Value = CashSalesItemsObj(i).Description4
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description5) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description5").Value = CashSalesItemsObj(i).Description5
                                End If
                                If Not IsNothing(CashSalesItemsObj(i).Description6) Then
                                    ARInvoiceDocument.Lines.UserFields.Fields.Item("U_Description6").Value = CashSalesItemsObj(i).Description6
                                End If
                                ARInvoiceDocument.Lines.WarehouseCode = CashSalesItemsObj(i).Location
                                ARInvoiceDocument.Lines.Quantity = CashSalesItemsObj(i).Qty
                                ARInvoiceDocument.Lines.UnitPrice = CashSalesItemsObj(i).UnitPrice
                                ARInvoiceDocument.Lines.VatGroup = CashSalesItemsObj(i).GstName
                                ARInvoiceDocument.Lines.Add()
                            Next
                        End Using
                        DocSuccess = ARInvoiceDocument.Add()
                        Dim DocType As String = "AR Invoice"
                        If DocSuccess = 0 Then

                            Dim SyncMessage As String = String.Empty
                            Dim DocEntry As Integer = 0
                            DocEntry = SAPCompany.GetNewObjectKey()
                            Dim DocNum As String = CURD.GetDocNum("OINV", DocEntry.ToString)

                            SyncMessage = "AR Invoice created SAP Doc No: " & DocNum & " Based on BMSS DocNum: " & CashSalesHeaderObj.DocNum
                            WriteSyncLog(DocType, "Succeeded", SyncMessage)
                            'AppSpecificFunc.UpdateSyncStaus("CashSalesDocH", True, DocNum, CashSalesHeaderObj.DocEntry, SyncMessage)

                            Dim PaymentErrCode As Integer = 0
                            Dim PaymentErrMsg As String = String.Empty
                            Dim PaymentDocSuccess As Long
                            Dim PaymentDocNum As String = String.Empty

                            Dim oPayment As SAPbobsCOM.Payments
                            oPayment = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)

                            oPayment.CardCode = CashSalesHeaderObj.CardCode
                            oPayment.CardName = CashSalesHeaderObj.CardCode

                            oPayment.DocDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.DueDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.TaxDate = ValidDocDate.ToString("yyyy-MM-dd")
                            oPayment.UserFields.Fields.Item("U_webref").Value = CashSalesHeaderObj.DocNum

                            oPayment.DocTypte = SAPbobsCOM.BoRcptTypes.rCustomer
                            oPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments

                            oPayment.Invoices.DocEntry = DocEntry
                            'Credit Card
                            For i As Integer = 0 To CashSalesPaymentsObj.Count - 1
                                oPayment.CreditCards.CreditCard = CashSalesPaymentsObj(i).PaymentType
                                oPayment.CreditCards.CreditAcct = CashSalesPaymentsObj(i).GLCode
                                oPayment.CreditCards.FirstPaymentSum = CashSalesPaymentsObj(i).PaidAmount
                                oPayment.CreditCards.CreditSum = CashSalesPaymentsObj(i).PaidAmount
                                oPayment.CreditCards.CreditCardNumber = "1111 1111 1111 1111"
                                oPayment.CreditCards.CardValidUntil = Date.Now.AddDays(10).ToString("yyyy-MM-dd")
                                oPayment.CreditCards.VoucherNum = "888"
                                'oPayment.CreditCards.UserFields.Fields.Item(" U_ChequeNoReference").Value = CashSalesPaymentsObj(i).ChequeNoReference
                                'oPayment.CreditCards.UserFields.Fields.Item(" U_PaymentRemarks").Value = CashSalesPaymentsObj(i).PaymentRemarks
                                oPayment.CreditCards.Add()
                            Next

                            ' Bank Transfer
                            'oPayment.TransferSum = OrderHeaderObj.OrderTotal
                            'oPayment.TransferAccount = 161016 ' My PC Local Account Code for Testing
                            'oPayment.TransferAccount = 121010

                            ' Cash
                            'oPayment.CashSum = OrderHeaderObj.OrderTotal
                            'oPayment.CashAccount = 161016
                            oPayment.JournalRemarks = JournalRemarks
                            PaymentDocSuccess = oPayment.Add()
                            If PaymentDocSuccess = 0 Then
                                Dim PaymentDocEntry As Integer = 0
                                PaymentDocEntry = SAPCompany.GetNewObjectKey()
                                PaymentDocNum = CURD.GetDocNum("ORCT", PaymentDocEntry.ToString)
                                SyncMessage = SyncMessage & ", AR Invoice Payment created SAP Doc No: " & PaymentDocNum & " Based on BMSS DocNum: " & CashSalesHeaderObj.DocNum
                                WriteSyncLog("Payment", "Succeeded", SyncMessage)
                                AppSpecificFunc.UpdateSyncStaus("CashSalesDocH", True, DocNum, CashSalesHeaderObj.DocEntry, SyncMessage, PaymentDocNum)


                                i_CashSalesDocH_Repository.WriteCSInventoryLogs(Lines, CashSalesHeaderObj.DocNum, "Sync Program", "CS - SAP Sync")
                                i_CashSalesDocH_Repository.UpdateStockBalance(Lines.Select(Function(x) x.ItemCode).ToList(), "CS - SAP Sync")
                                i_CashSalesDocH_Repository.CommitChanges()


                            Else
                                Dim ErrCode As Integer = 0
                                Dim ErrMsg As String = String.Empty
                                SAPCompany.GetLastError(ErrCode, ErrMsg)
                                Dim SyncErrMessage As String = String.Empty
                                SyncErrMessage = "Payment creation failed for Cash Sales: " & CashSalesHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                                WriteSyncLog(DocType, "Failed", SyncErrMessage)
                                AppSpecificFunc.UpdateSyncStaus("CashSalesDocH", False, "", CashSalesHeaderObj.DocEntry, SyncErrMessage)
                            End If
                        Else
                            Dim ErrCode As Integer = 0
                            Dim ErrMsg As String = String.Empty
                            SAPCompany.GetLastError(ErrCode, ErrMsg)
                            Dim SyncMessage As String = String.Empty
                            SyncMessage = "AR Invoice creation failed for DO: " & CashSalesHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                            WriteSyncLog(DocType, "Failed", SyncMessage)
                            AppSpecificFunc.UpdateSyncStaus("CashSalesDocH", False, "", CashSalesHeaderObj.DocEntry, SyncMessage)
                        End If
                        SAPCompany.Disconnect()
                        SAPCompany = Nothing
                    Else

                    End If
                End If
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_Change_CR_Limit()
        Try
            Dim ChangCRLObj As New BMSS_Sync_Program.Models.ChangeCRLimit

            If LoadCRLimitFromDataBase(ChangCRLObj) Then
                Dim SAPCompany As New SAPbobsCOM.Company

                GetSAPCompanyObject(SAPCompany)
                If Not IsNothing(SAPCompany) Then
                    Dim BPDocSuccess As Long

                    Dim BP As SAPbobsCOM.BusinessPartners
                    BP = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners)

                    If BP.GetByKey(ChangCRLObj.CardCode) Then
                        BP.MaxCommitment = ChangCRLObj.NewLimit
                        BP.CreditLimit = ChangCRLObj.NewLimit
                    End If
                    BPDocSuccess = BP.Update()
                    If BPDocSuccess = 0 Then
                        Dim UQ As New UpdateQuery
                        Dim inputObj As New BMSS_Sync_Program.Models.ChangeCRLimit
                        inputObj.SubmitToSAP = 0
                        inputObj.SubmittedToSAPOn = Format(CDate(Date.Now), "yyyy-MM-dd HH:mm:ss")

                        Dim FilterObj As New BMSS_Sync_Program.Models.ChangeCRLimit
                        FilterObj.CardCode = ChangCRLObj.CardCode

                        UQ._InputTable = inputObj
                        UQ._FilterTable = FilterObj

                        'Query Condition builder
                        Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

                        'Query Condition Groups
                        Dim ConditionsGrp1 As List(Of String) = New List(Of String)

                        'Query Conditions values

                        ConditionsGrp1.Add("CardCode=@Filter_CardCode")

                        If ConditionsGrp1.Count > 0 Then
                            QryConditions.Add(" AND ", ConditionsGrp1)
                        End If

                        UQ._HasInBetweenConditions = False
                        UQ._HasWhereConditions = True
                        UQ._DB = "Custom"
                        UQ._Conditions = QryConditions

                        If CURD.UpdateData(UQ) = False Then

                        End If
                    Else
                        Dim UQ As New UpdateQuery
                        Dim inputObj As New BMSS_Sync_Program.Models.ChangeCRLimit
                        inputObj.SubmitToSAP = 0


                        Dim FilterObj As New BMSS_Sync_Program.Models.ChangeCRLimit
                        FilterObj.CardCode = ChangCRLObj.CardCode

                        UQ._InputTable = inputObj
                        UQ._FilterTable = FilterObj

                        'Query Condition builder
                        Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

                        'Query Condition Groups
                        Dim ConditionsGrp1 As List(Of String) = New List(Of String)

                        'Query Conditions values

                        ConditionsGrp1.Add("CardCode=@Filter_CardCode")

                        If ConditionsGrp1.Count > 0 Then
                            QryConditions.Add(" AND ", ConditionsGrp1)
                        End If

                        UQ._HasInBetweenConditions = False
                        UQ._HasWhereConditions = True
                        UQ._DB = "Custom"
                        UQ._Conditions = QryConditions

                        If CURD.UpdateData(UQ) = False Then

                        End If

                        Dim ErrCode As Integer = 0
                        Dim ErrMsg As String = String.Empty
                        SAPCompany.GetLastError(ErrCode, ErrMsg)
                        Dim SyncErrMessage As String = String.Empty
                        SyncErrMessage = "Credit Limit Change failed for CardCode: " & ChangCRLObj.CardCode & ",  sync error : " & ErrCode & "-" & ErrMsg
                        WriteSyncLog("CreditLimit", "Failed", SyncErrMessage)
                    End If

                    SAPCompany.Disconnect()
                    SAPCompany = Nothing
                End If
            End If
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Sub Func_Payment()
        Try
            Dim InvoiceHeaderObj As New DODocH
            'If Not LoadHeaderFromDataBase(InvoiceHeaderObj) Then 'If There is no pending DO for syncing then sync payment documents
            Using i_DODocH_Repository As I_DODocH_Repository = New EF_DODocHeader_Repository()
                InvoiceHeaderObj = i_DODocH_Repository.GetDocumentWaitForSyncing()
                If (IsNothing(InvoiceHeaderObj)) Then
                    Dim PaymentHeaderObj As New BMSS_Sync_Program.Models.PaymentDocH
                    Dim PaymentItemsObj As BMSS_Sync_Program.Models.PaymentDocLs() = Nothing
                    If LoadPaymentHeaderFromDataBase(PaymentHeaderObj) Then

                        PaymentItemsObj = LoadPaymentItemsFromDataBase(PaymentHeaderObj.DocEntry)
                        Dim ListofDocEntry As New List(Of BMSS_Sync_Program.Models.OINV)
                        Dim SubmittedDO As Integer = LoadPaymentItemsSubmittedDOFromDataBase(PaymentHeaderObj.DocEntry, ListofDocEntry)
                        If Not SubmittedDO.Equals(PaymentItemsObj.Length) Then

                        Else

                            Dim SAPCompany As New SAPbobsCOM.Company

                            GetSAPCompanyObject(SAPCompany)
                            If Not IsNothing(SAPCompany) Then
                                Dim PaymentErrCode As Integer = 0
                                Dim PaymentErrMsg As String = String.Empty
                                Dim PaymentDocSuccess As Long
                                Dim PaymentDocNum As String = String.Empty

                                Dim oPayment As SAPbobsCOM.Payments
                                oPayment = SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)

                                oPayment.CardCode = PaymentHeaderObj.CardCode
                                oPayment.CardName = PaymentHeaderObj.CardName

                                oPayment.DocTypte = SAPbobsCOM.BoRcptTypes.rCustomer
                                oPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments


                                oPayment.UserFields.Fields.Item("U_webref").Value = PaymentHeaderObj.DocNum



                                Dim PaidAmountTotal As Decimal = 0
                                Dim DiscountAmountTotal As Decimal = 0
                                For i As Integer = 0 To PaymentItemsObj.Count - 1
                                    PaidAmountTotal = PaidAmountTotal + PaymentItemsObj(i).PaymentAmount
                                    DiscountAmountTotal = DiscountAmountTotal + PaymentItemsObj(i).DiscountAmount
                                    If (PaymentItemsObj(i).PaymentAmount > 0) Then
                                        oPayment.Invoices.DocEntry = ListofDocEntry(i).DocEntry
                                        oPayment.Invoices.InvoiceType = 13
                                        oPayment.Invoices.SumApplied = PaymentItemsObj(i).PaymentAmount
                                        'oPayment.Invoices.TotalDiscount = PaymentItemsObj(i).DiscountAmount
                                        oPayment.Invoices.Add()
                                    End If

                                Next

                                oPayment.CreditCards.CreditCard = PaymentHeaderObj.PaymentType
                                oPayment.CreditCards.CreditAcct = PaymentHeaderObj.GLCode
                                'oPayment.CreditCards.FirstPaymentSum = PaidAmountTotal ' 1000.000
                                'oPayment.CreditCards.CreditSum = PaidAmountTotal

                                oPayment.CreditCards.FirstPaymentSum = PaymentHeaderObj.PaidAmount ' 1000.000
                                oPayment.CreditCards.CreditSum = PaymentHeaderObj.PaidAmount

                                oPayment.CreditCards.CreditCardNumber = "1111 1111 1111 1111"
                                oPayment.CreditCards.CardValidUntil = Date.Now.AddDays(10).ToString("yyyy-MM-dd")
                                oPayment.CreditCards.VoucherNum = "888"
                                oPayment.CreditCards.Add()

                                If (DiscountAmountTotal <> 0) Then

                                    oPayment.CreditCards.CreditCard = "6"
                                    'oPayment.CreditCards.CreditAcct = PaymentHeaderObj.GLCode
                                    oPayment.CreditCards.FirstPaymentSum = DiscountAmountTotal * -1 ' 1000.000
                                    oPayment.CreditCards.CreditSum = DiscountAmountTotal * -1
                                    oPayment.CreditCards.CreditCardNumber = "1111 1111 1111 1111"
                                    oPayment.CreditCards.CardValidUntil = Date.Now.AddDays(10).ToString("yyyy-MM-dd")
                                    oPayment.CreditCards.VoucherNum = "888"

                                End If

                                ' Bank Transfer
                                'oPayment.TransferSum = OrderHeaderObj.OrderTotal
                                'oPayment.TransferAccount = 161016 ' My PC Local Account Code for Testing
                                'oPayment.TransferAccount = 121010

                                ' Cash
                                'oPayment.CashSum = OrderHeaderObj.OrderTotal
                                'oPayment.CashAccount = 161016

                                PaymentDocSuccess = oPayment.Add()
                                If PaymentDocSuccess = 0 Then
                                    Dim PaymentDocEntry As Integer = 0
                                    PaymentDocEntry = SAPCompany.GetNewObjectKey()
                                    PaymentDocNum = CURD.GetDocNum("ORCT", PaymentDocEntry.ToString)
                                    Dim SyncMessage As String = " Payment created SAP Doc No: " & PaymentDocNum & " Based on BMSS DocNum: " & PaymentHeaderObj.DocNum
                                    WriteSyncLog("Payment", "Succeeded", SyncMessage)
                                    AppSpecificFunc.UpdateSyncStaus("PaymentDocH", True, PaymentDocNum, PaymentHeaderObj.DocEntry, SyncMessage)
                                Else
                                    Dim ErrCode As Integer = 0
                                    Dim ErrMsg As String = String.Empty
                                    SAPCompany.GetLastError(ErrCode, ErrMsg)
                                    Dim SyncErrMessage As String = String.Empty
                                    SyncErrMessage = "Payment creation failed for Cash Sales: " & PaymentHeaderObj.DocNum & ",  sync error : " & ErrCode & "-" & ErrMsg
                                    WriteSyncLog("Payment", "Failed", SyncErrMessage)
                                    AppSpecificFunc.UpdateSyncStaus("PaymentDocH", False, "", PaymentHeaderObj.DocEntry, SyncErrMessage)
                                End If
                                SAPCompany.Disconnect()
                                SAPCompany = Nothing
                            End If
                        End If

                    End If
                End If
            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
    Protected Function LoadCRLimitFromDataBase(ByRef Obj As BMSS_Sync_Program.Models.ChangeCRLimit) As Boolean

        Try
            Dim ReturnResult As Boolean = False

            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            Obj.SubmitToSAP = "1"

            SQ._InputTable = Obj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("SubmitToSAP=@SubmitToSAP")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(Obj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                ReturnResult = True
            Else
                ReturnResult = False
            End If
            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function


    Protected Function LoadHeaderFromDataBase(ByRef Obj As Object, Optional DocEntry As String = "0") As Boolean

        Try


            Dim ReturnResult As Boolean = False

            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            Obj.SubmittedToSAP = "1"
            If DocEntry <> "0" Then
                Obj.DocEntry = DocEntry
            End If


            SQ._InputTable = Obj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("SubmittedToSAP=@SubmittedToSAP")
            If DocEntry <> "0" Then
                ConditionsGrp1.Add("DocEntry=@DocEntry")
            End If
            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)




            If ResultDataTable.Rows.Count > 0 Then
                AppSpecificFunc.DataTableToObject(Obj, ResultDataTable)
                ReturnResult = True
            Else
                ReturnResult = False
            End If
            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return False
        End Try
    End Function
    'Protected Function LoadPaymentHeaderFromDataBase(ByRef Obj As Object) As Boolean
    '    Try
    '        Dim ReturnResult As Boolean = False
    '        Dim ResultDataTable As New DataTable


    '        Dim CQ As New CustomQuery

    '        CQ._DB = "Custom"
    '        Dim CustomQueryParameters As New Dictionary(Of String, String)
    '        Dim InputQuery1 As String = String.Empty
    '        InputQuery1 = " SELECT Top 1 T3.* FROM PaymentDocLs T1 INNER JOIN DODocH T2 ON T1.ReferenceDocNum = T2.DocNum " &
    '                       " INNER JOIN PaymentDocH T3 ON T1.DocEntry = T3.DocEntry "
    '        Dim Conditionlist1 As New List(Of String)


    '        Conditionlist1.Add(" T2.SAPDocNum IS NOT NULL ")
    '        Conditionlist1.Add(" T3.SubmittedToSAP=@SubmittedToSAP")
    '        CustomQueryParameters.Add("@SubmittedToSAP", 1)

    '        If Conditionlist1.Count > 0 Then
    '            Dim CondiString1 As String = String.Join(" AND ", Conditionlist1)
    '            InputQuery1 = InputQuery1 & " WHERE " & CondiString1
    '        End If

    '        **********************Query Builder Function *****************
    '        CQ._InputQuery = InputQuery1 & ""
    '        CQ._Parameters = CustomQueryParameters

    '        ResultDataTable = CURD.CustomQueryGetData(CQ)
    '        If ResultDataTable.Rows.Count > 0 Then
    '            AppSpecificFunc.DataTableToObject(Obj, ResultDataTable)
    '            ReturnResult = True
    '        Else
    '            ReturnResult = False
    '        End If

    '        Return ReturnResult
    '    Catch ex As Exception
    '        Return False
    '    End Try
    'End Function

    Protected Function LoadPaymentHeaderFromDataBase(ByRef Obj As Object) As Boolean
        Try
            Dim ReturnResult As Boolean = False
            Dim ResultDataTable As New DataTable


            Dim CQ As New CustomQuery

            CQ._DB = "Custom"
            Dim CustomQueryParameters As New Dictionary(Of String, String)
            Dim InputQuery1 As String = String.Empty
            InputQuery1 = " SELECT T1.DocEntry FROM PaymentDocLs T1 LEFT JOIN DODocH T2 ON T1.ReferenceDocNum = T2.DocNum " &
                           " INNER JOIN PaymentDocH T3 ON T1.DocEntry = T3.DocEntry    "
            Dim Conditionlist1 As New List(Of String)


            Conditionlist1.Add(" T3.SyncStatus=@SyncStatus")
            Conditionlist1.Add(" T3.SubmittedToSAP=@SubmittedToSAP")
            CustomQueryParameters.Add("@SubmittedToSAP", 1)
            CustomQueryParameters.Add("@SyncStatus", 1)

            If Conditionlist1.Count > 0 Then
                Dim CondiString1 As String = String.Join(" AND ", Conditionlist1)
                InputQuery1 = InputQuery1 & " WHERE " & CondiString1
            End If

            '**********************Query Builder Function *****************
            CQ._InputQuery = InputQuery1 & " Group By T1.DocEntry HAVING COUNT(T2.SAPDocNum)=COUNT(T1.LineNum)"
            CQ._Parameters = CustomQueryParameters

            ResultDataTable = CURD.CustomQueryGetData(CQ)
            If ResultDataTable.Rows.Count > 0 Then
                LoadHeaderFromDataBase(Obj, ResultDataTable.Rows(0).Item(0))
                ReturnResult = True
            Else
                ReturnResult = False
            End If

            Return ReturnResult
        Catch ex As Exception
            Return False
        End Try
    End Function
    Protected Function LoadPaymentItemsSubmittedDOFromDataBase(DocEntry As String, ByRef ListOfDODocEntry As List(Of BMSS_Sync_Program.Models.OINV)) As Integer

        Try

            Dim ReturnResult As Integer = 0
            Dim ResultDataTable As New DataTable


            Dim CQ As New CustomQuery

            CQ._DB = "Custom"
            Dim CustomQueryParameters As New Dictionary(Of String, String)
            Dim InputQuery1 As String = String.Empty
            InputQuery1 = " SELECT T2.* FROM PaymentDocLs T1 INNER JOIN DODocH T2 ON T1.ReferenceDocNum = T2.DocNum "
            Dim Conditionlist1 As New List(Of String)


            Conditionlist1.Add(" T2.SAPDocNum IS NOT NULL ")
            Conditionlist1.Add(" T1.DocEntry=@DocEntry")
            CustomQueryParameters.Add("@DocEntry", DocEntry)

            If Conditionlist1.Count > 0 Then
                Dim CondiString1 As String = String.Join(" AND ", Conditionlist1)
                InputQuery1 = InputQuery1 & " WHERE " & CondiString1
            End If

            '**********************Query Builder Function *****************
            CQ._InputQuery = InputQuery1 & ""
            CQ._Parameters = CustomQueryParameters

            ResultDataTable = CURD.CustomQueryGetData(CQ)
            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim DODocHObject As New DODocH
                    AppSpecificFunc.DataRowToObject(DODocHObject, DR)
                    Dim InvoiceDocEntry As BMSS_Sync_Program.Models.OINV = GetARInvoiceInventory(DODocHObject.SAPDocNum)
                    ListOfDODocEntry.Add(InvoiceDocEntry)

                    i = i + 1
                Next
            End If
            ReturnResult = ResultDataTable.Rows.Count

            Return ReturnResult
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return 0
        End Try
    End Function
    Protected Function GetARInvoiceInventory(DocNum As String) As BMSS_Sync_Program.Models.OINV
        Dim OINVObj As New BMSS_Sync_Program.Models.OINV
        Try


            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            OINVObj.DocNum = DocNum

            SQ._InputTable = OINVObj
            SQ._DB = "SAP"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocNum=@DocNum")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(OINVObj, ResultDataTable)

            Return OINVObj

        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return OINVObj
        End Try

    End Function
    Protected Function LoadPaymentItemsFromDataBase(DocEntry As String) As BMSS_Sync_Program.Models.PaymentDocLs()

        Try
            Dim ChildItems As BMSS_Sync_Program.Models.PaymentDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New BMSS_Sync_Program.Models.PaymentDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True


            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New BMSS_Sync_Program.Models.PaymentDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadDOItemsFromDataBase(DocEntry As String) As DODocLs()

        Try
            Dim ChildItems As DODocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New DODocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New DODocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadCashSalesItemsFromDataBase(DocEntry As String) As CashSalesDocLs()

        Try
            Dim ChildItems As CashSalesDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New CashSalesDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New CashSalesDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadCashSalesPaymentsFromDataBase(DocEntry As String) As CashSalesDocPays()

        Try
            Dim ChildItems As CashSalesDocPays() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New CashSalesDocPays
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New CashSalesDocPays
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadCashSalesCreditItemsFromDataBase(DocEntry As String) As CashSalesCreditDocLs()

        Try
            Dim ChildItems As CashSalesCreditDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New CashSalesCreditDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New CashSalesCreditDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadCashSalesCreditPaymentsFromDataBase(DocEntry As String) As CashSalesCreditDocPays()

        Try
            Dim ChildItems As CashSalesCreditDocPays() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New CashSalesCreditDocPays
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New CashSalesCreditDocPays
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function

    Protected Function LoadStockTransferItemsFromDataBase(DocEntry As String) As StockTransDocLs()
        Try

            Dim ChildItems As StockTransDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New StockTransDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New StockTransDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadStockReceiptItemsFromDataBase(DocEntry As String) As StockReceiptDocLs()
        Try

            Dim ChildItems As StockReceiptDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New StockReceiptDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New StockReceiptDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadStockIssueItemsFromDataBase(DocEntry As String) As StockIssueDocLs()
        Try

            Dim ChildItems As StockIssueDocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New StockIssueDocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"


            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New StockIssueDocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Protected Function LoadGRPOItemsFromDataBase(DocEntry As String) As GRPODocLs()

        Try
            Dim ChildItems As GRPODocLs() = Nothing
            Dim ReturnResult As Boolean = False

            Dim ItemsObj As New GRPODocLs
            Dim ErrMsg As String = String.Empty
            Dim SQ As New SelectQuery

            'Where Condition parameter
            ItemsObj.DocEntry = DocEntry

            SQ._InputTable = ItemsObj
            SQ._DB = "Custom"
            SQ._HasInBetweenConditions = False
            SQ._HasWhereConditions = True

            'Query Conditions List
            Dim QryConditions As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))

            'Query Condition Groups
            Dim ConditionsGrp1 As List(Of String) = New List(Of String)

            'Query Conditions values
            ConditionsGrp1.Add("DocEntry=@DocEntry")

            QryConditions.Add(" AND ", ConditionsGrp1)

            SQ._Conditions = QryConditions
            SQ._OrderBy = " LineNum ASC"

            Dim ResultDataTable As New DataTable
            ResultDataTable = CURD.SelectAllData(SQ)

            AppSpecificFunc.DataTableToObject(ItemsObj, ResultDataTable)


            If ResultDataTable.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each DR As DataRow In ResultDataTable.Rows
                    Dim ItemsNewObject As New GRPODocLs
                    AppSpecificFunc.DataRowToObject(ItemsNewObject, DR)
                    ReDim Preserve ChildItems(i)
                    ChildItems(i) = ItemsNewObject
                    i = i + 1
                Next
            End If

            Return ChildItems
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
            Return Nothing
        End Try
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Process_Timer.Enabled = True

            Using i_OpeningBalance_Repository As I_OpeningBalance_Repository = New EF_OpeningBalance_Repository()

                Dim SetOpeningBalance As Boolean = i_OpeningBalance_Repository.CheckOpeningBalanceStatus()

                If (SetOpeningBalance = True) Then
                    Using i_StockReceiptDocH_Repository As I_StockReceiptDocH_Repository = New EF_StockReceiptDocHeader_Repository()
                        i_StockReceiptDocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "SR - Opening Balance")
                        i_StockReceiptDocH_Repository.CommitChanges()
                    End Using
                    Using i_StockIssueDocH_Repository As I_StockIssueDocH_Repository = New EF_StockIssueDocHeader_Repository()
                        i_StockIssueDocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "SI - Opening Balance")
                        i_StockIssueDocH_Repository.CommitChanges()
                    End Using
                    Using i_StockTransDocH_Repository As I_StockTransDocH_Repository = New EF_StockTransDocHeader_Repository()
                        i_StockTransDocH_Repository.UpdateStockBalanceItems(i_OpeningBalance_Repository.ItemList, "ST - Opening Balance")
                        i_StockTransDocH_Repository.CommitChanges()
                    End Using
                    Using i_GRPODocH_Repository As I_GRPODocH_Repository = New EF_GRPODocHeader_Repository()
                        i_GRPODocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "GRPO - Opening Balance")
                        i_GRPODocH_Repository.CommitChanges()
                    End Using
                    Using i_DODocH_Repository As I_DODocH_Repository = New EF_DODocHeader_Repository()
                        i_DODocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "DO - Opening Balance")
                        i_DODocH_Repository.CommitChanges()
                    End Using
                    Using i_CashSalesCreditDocH_Repository As I_CashSalesCreditDocH_Repository = New EF_CashSalesCreditDocHeader_Repository()
                        i_CashSalesCreditDocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "Cash Sales credit - Opening Balance")
                        i_CashSalesCreditDocH_Repository.CommitChanges()
                    End Using
                    Using i_CashSalesDocH_Repository As I_CashSalesDocH_Repository = New EF_CashSalesDocHeader_Repository()
                        i_CashSalesDocH_Repository.UpdateStockBalance(i_OpeningBalance_Repository.ItemList, "Cash Sales - Opening Balance")
                        i_CashSalesDocH_Repository.CommitChanges()
                    End Using
                    i_OpeningBalance_Repository.ResetOpeningBalanceStatus()

                End If

            End Using
        Catch ex As Exception
            AppSpecificFunc.WriteLog(ex)
        End Try
    End Sub
End Class
