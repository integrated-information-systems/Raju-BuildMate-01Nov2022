Namespace Models
    Public Enum Processes As Integer
        Process_Syncing_AR_Invoice = 1
        Process_Syncing_AR_Invoice_And_Incoming_Payment = 2
        Process_Syncing_AR_Invoice_And_Outgoing_Payment = 3
        'GRPO
        Process_Syncing_Goods_Receipt_PO = 4
        Process_Syncing_Goods_Issue = 5
        Process_Syncing_Goods_Receipt = 6
        Process_Syncing_Goods_Transfer = 7
        Process_Syncing_Payment = 8
        Process_Syncing_Change_CR_Limit = 9
    End Enum
End Namespace

