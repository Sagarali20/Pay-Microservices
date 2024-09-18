using Common;

namespace RequestMoneyService.Models
{
    public class RequestMoney : CommonProperty
    {
        public string TxSenderPhone { get; set; }
        public string TxReceiverPhone { get; set; }
        public decimal DccAmount { get; set; }
        public decimal DccChargeAmount { get; set; }
        public string TxReferenceNote { get; set; }
    }
}
