using Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RemittanceService.Application.Request.Ramittance.Command
{
    public class RamittanceInformation : IRequest<Result>
    {
        
        public int RemittanceKey { get; set; }
        public int RemittanceNo { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiarySurname { get; set; }
        public int BeneficiaryCountryId { get; set; }
        public string BeneficiaryAccIdentifier { get; set; }
        public string BeneficiaryCurrencyCode { get; set; }
        public decimal BeneficiaryAmount { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerGender { get; set; }
        public string CustomerPassportNo { get; set; }
        public int CustomerPassportCountryId { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerSuburb { get; set; }
        public int CustomerCityId { get; set; }
        public string CustomerAccNo { get; set; }
        public string CustomerPhone { get; set; }
        public decimal CustomerAmount { get; set; }
        public string ReferenceNo { get; set; }
        public int TransactionExtTypeKey { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }

        public RamittanceInformation(
            int remittanceKey, int remittanceNo, string beneficiaryName, string beneficiarySurname,
            int beneficiaryCountryId, string beneficiaryAccIdentifier, string beneficiaryCurrencyCode, 
            decimal beneficiaryAmount, int customerId, string customerName, string customerSurname, 
            string customerGender, string customerPassportNo, int customerPassportCountryId, string customerAddress,
            string customerSuburb, int customerCityId, string customerAccNo, string customerPhone, 
            decimal customerAmount, string referenceNo, int transactionExtTypeKey, string description, int isActive
            )
        {
            RemittanceKey = remittanceKey;
            RemittanceNo = remittanceNo;
            BeneficiaryName = beneficiaryName;
            BeneficiarySurname = beneficiarySurname;
            BeneficiaryCountryId = beneficiaryCountryId;
            BeneficiaryAccIdentifier = beneficiaryAccIdentifier;
            BeneficiaryCurrencyCode = beneficiaryCurrencyCode;
            BeneficiaryAmount = beneficiaryAmount;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerSurname = customerSurname;
            CustomerGender = customerGender;
            CustomerPassportNo = customerPassportNo;
            CustomerPassportCountryId = customerPassportCountryId;
            CustomerAddress = customerAddress;
            CustomerSuburb = customerSuburb;
            CustomerCityId = customerCityId;
            CustomerAccNo = customerAccNo;
            CustomerPhone = customerPhone;
            CustomerAmount = customerAmount;
            ReferenceNo = referenceNo;
            TransactionExtTypeKey = transactionExtTypeKey;
            Description = description;
            IsActive = isActive;
        }
    }

    public class RamittanceInformationHandler : IRequestHandler<RamittanceInformation, Result>
    {
        private readonly IRamittanceService _ramittanceService;
        private readonly ILogger<RamittanceInformationHandler> _logger;
        public RamittanceInformationHandler(IRamittanceService ramittanceService, ILogger<RamittanceInformationHandler> logger)
        {
            _ramittanceService = ramittanceService;
            _logger = logger;
        }
        public async Task<Result> Handle(RamittanceInformation request, CancellationToken cancellationToken)
        {
            Models.Ramittance ramittance = new Models.Ramittance
            {
                IdRemittanceKey = request.RemittanceKey,
                IdRemittanceNo = request.RemittanceNo,
                TxBeneficiaryName = request.BeneficiaryName,
                TxBeneficiarySurname = request.BeneficiarySurname,
                TxBeneficiaryAccIdentifier = request.BeneficiaryAccIdentifier,
                TxBeneficiaryCurrencyCode = request.BeneficiaryCurrencyCode,
                IdBeneficiaryCountry = request.BeneficiaryCountryId,
                DecBeneficiaryAmount = request.BeneficiaryAmount,
                IdCustomerId = request.CustomerId,
                TxCustomerName = request.CustomerName,
                TxCustomerSurname = request.CustomerSurname,
                TxCustomerGender = request.CustomerGender,
                TxCustomerPassportNo = request.CustomerPassportNo,
                IdCustomerPassportCountry = request.CustomerPassportCountryId,
                TxCustomerAddress = request.CustomerAddress,
                TxCustomerSuburb = request.CustomerSuburb,
                IdCustomerCity = request.CustomerCityId,
                TxCustomerAccNo = request.CustomerAccNo,
                TxCustomerPhone = request.CustomerPhone,
                DccCustomerAmount = request.CustomerAmount,
                TxReferenceNo = request.ReferenceNo,
                IdTransactionExtTypeKey = request.TransactionExtTypeKey,
                TxDescription = request.Description,
                IsActive = request.IsActive
            };

            _logger.LogInformation("request from remittance handler");
            var result = await _ramittanceService.InsertRamittanceInfo(ramittance);
            _logger.LogInformation("success from remittance handler");
            return result;
        }
    }


}
