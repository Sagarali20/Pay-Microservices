namespace SendMoneyService.Models
{
    public class Account
    {
        public int IdAccountKey { get; set; }
        public int IdAccountVer { get; set; }
        public int IdUserKey { get; set; }
        public int IdAccountType { get; set; }
        public string TxAccountNumber { get; set; }=string.Empty;
        public decimal DecBalance { get; set; }
    }
}
