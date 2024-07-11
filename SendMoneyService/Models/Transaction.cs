namespace SendMoneyService.Models
{
    public class Transaction
    {
        public int IdTransactionKey { get; set; }
        public int IdTransactionVer { get; set; }
        public int IdTransactionTypeKey { get; set; }
        public decimal DecTransactionAmount { get; set; }
        public string TxTransactionStatus { get; set; } = string.Empty;
        public DateTime DttCreated { get; set; }
        public int IdTransactionExtTypeKey { get; set; }
    }
}
