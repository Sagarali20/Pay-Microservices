using Common;
using MediatR;

namespace RemittanceService.Application.Request.Ramittance.Command
{
    public class RamittanceInformation : IRequest<Result>
    {
        public int IdRemittanceKey { get; set; }
        public int IdRemittanceNo { get; set; }
        public string TxBeneficiaryName { get; set; }
        public string TxBeneficiarySurname { get; set; }
        public string TxBeneficiaryCountry { get; set; }
        public string TxBeneficiaryAccIdentifier { get; set; }
        public string TxBeneficiaryCurrencyCode { get; set; }
        public decimal DecBeneficiaryAmount { get; set; }
        public int IdCustomerId { get; set; }
        public string TxCustomerName { get; set; }
        public string TxCustomerSurname { get; set; }
        public string TxCustomerGender { get; set; }
        public string TxCustomerPassportNo { get; set; }
        public string TxCustomerPassportCountry { get; set; }
        public string TxCustomerAddress { get; set; }
        public string TxCustomerSuburb { get; set; }
        public string TxCustomerCity { get; set; }
        public string TxCustomerAccNo { get; set; }
        public string TxCustomerPhone { get; set; }
        public decimal DccCustomerAmount { get; set; }
        public string TxReferenceNo { get; set; }
        public int IdTransactionExtTypeKey { get; set; }
        public string TxDescription { get; set; }
        public int IsActive { get; set; }

        public RamittanceInformation(int idRemittanceKey, int idRemittanceNo, string txBeneficiaryName, string txBeneficiarySurname,
            string txBeneficiaryCountry, string txBeneficiaryAccIdentifier, string txBeneficiaryCurrencyCode, decimal decBeneficiaryAmount, int idCustomerId,
            string txCustomerName, string txCustomerSurname, string txCustomerGender, string txCustomerPassportNo, string txCustomerPassportCountry,
            string txCustomerAddress, string txCustomerSuburb, string txCustomerCity, string txCustomerAccNo, string txCustomerPhone, decimal dccCustomerAmount,
            string txReferenceNo, int idTransactionExtTypeKey, string txDescription, int isActive
            )
        {
            IdRemittanceKey = idRemittanceKey;
            IdRemittanceNo = idRemittanceNo;
            TxBeneficiaryName = txBeneficiaryName;
            TxBeneficiarySurname = txBeneficiarySurname;
            TxBeneficiaryCountry = txBeneficiaryCountry;
            TxBeneficiaryAccIdentifier = txBeneficiaryAccIdentifier;
            TxBeneficiaryCurrencyCode = txBeneficiaryCurrencyCode;
            DecBeneficiaryAmount = decBeneficiaryAmount;
            IdCustomerId = idCustomerId;
            TxCustomerName = txCustomerName;
            TxCustomerSurname = txCustomerSurname;
            TxCustomerGender = txCustomerGender;
            TxCustomerPassportNo = txCustomerPassportNo;
            TxCustomerPassportCountry = txCustomerPassportCountry;
            TxCustomerAddress = txCustomerAddress;
            TxCustomerSuburb = txCustomerSuburb;
            TxCustomerCity = txCustomerCity;
            TxCustomerAccNo = txCustomerAccNo;
            TxCustomerPhone = txCustomerPhone;
            DccCustomerAmount = dccCustomerAmount;
            TxReferenceNo = txReferenceNo;
            IdTransactionExtTypeKey = idTransactionExtTypeKey;
            TxDescription = txDescription;
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
                IdRemittanceKey = request.IdRemittanceKey,
                IdRemittanceNo = request.IdRemittanceNo,
                TxBeneficiaryName = request.TxBeneficiaryName,
                TxBeneficiarySurname = request.TxBeneficiarySurname,
                TxBeneficiaryAccIdentifier = request.TxBeneficiaryAccIdentifier,
                TxBeneficiaryCurrencyCode = request.TxBeneficiaryCurrencyCode,
                TxBeneficiaryCountry = request.TxBeneficiaryCountry,
                DecBeneficiaryAmount = request.DecBeneficiaryAmount,
                TxCustomerName = request.TxCustomerName,
                TxCustomerSurname = request.TxCustomerSurname,
                TxCustomerGender = request.TxCustomerGender,
                TxCustomerPassportNo = request.TxCustomerPassportNo,
                TxCustomerPassportCountry = request.TxCustomerPassportCountry,
                TxCustomerAddress = request.TxCustomerAddress,
                TxCustomerSuburb = request.TxCustomerSuburb,
                TxCustomerCity = request.TxCustomerCity,
                TxCustomerAccNo = request.TxCustomerAccNo,
                TxCustomerPhone = request.TxCustomerPhone,
                DccCustomerAmount = request.DccCustomerAmount,
                TxReferenceNo = request.TxReferenceNo,
                IdTransactionExtTypeKey = request.IdTransactionExtTypeKey,
                TxDescription = request.TxDescription,
                IsActive = request.IsActive
            };

            _logger.LogInformation("request from remittance handler");
            var result = await _ramittanceService.InsertRamittanceInfo(ramittance);
            _logger.LogInformation("success from remittance handler");
            return result;
        }
    }


}
