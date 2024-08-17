namespace PaymentService.Models
{
    public class Transfer
    {
        public int IdTransferKey { get; set; }
        public int IdTransferVer { get; set; }
        public int IdTransactionKey { get; set; }
        public int IdTransactionChargeKey { get; set; }
        public int IdSenderKey { get; set; }
        public int IdReceiverKey { get; set; }
        public int IdCurrencyKey { get; set; }
        public string TxTransferMsg { get; set; } = string.Empty;
    }
}
