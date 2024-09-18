using Common;

namespace RemittanceService.Models
{
    public class Ramittance: CommonProperty
    {
        public int IdRemittanceKey { get; set; }
        public int IdRemittanceNo { get; set; }
        
        public string TxBeneficiaryName { get; set; }
        public string TxBeneficiarySurname { get; set; }
        public int IdBeneficiaryCountry { get; set; }
        public string TxBeneficiaryAccIdentifier { get; set; }
        public string TxBeneficiaryCurrencyCode { get; set; }
        public decimal DecBeneficiaryAmount { get; set; }

        public int IdCustomerId { get; set; }
        public string TxCustomerName { get; set; }
        public string TxCustomerSurname { get; set; }
        public string TxCustomerGender { get; set; }
        public string TxCustomerPassportNo { get; set; }
        public int IdCustomerPassportCountry { get; set; }
        public string TxCustomerAddress { get; set; }
        public string TxCustomerSuburb { get; set; }
        public int IdCustomerCity { get; set; }
        public string TxCustomerAccNo { get; set; }
        public string TxCustomerPhone { get; set; }
        public decimal DccCustomerAmount { get; set; }
        public string TxReferenceNo { get; set; }
        public int IdTransactionExtTypeKey { get; set; }
        public string TxDescription { get; set; }
        public int IsActive { get; set; }
    }
}
