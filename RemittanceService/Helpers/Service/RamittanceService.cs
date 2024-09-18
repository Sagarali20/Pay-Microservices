using System.Data;
using Dapper;
using RemittanceService.Application.Request.Ramittance;
using System.Transactions;
using Common;
using Common.Interface;
using System.Data.SqlClient;

namespace RemittanceService.Helpers.Service
{
    public class RamittanceService : IRamittanceService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDbConnection _connection;
        private readonly ILogger<RamittanceService> _logger;
        #region sp_parameter

        private static string ID_REMITTANCE_KEY = "@id_remittance_key";
        private static string ID_REMITTANCE_NO = "@id_remittance_no";

        private static string TX_BENEFICIARY_NAME = "@tx_beneficiaryName";
        private static string TX_BENEFICIARY_SURENAME = "@tx_beneficiarySurname";
        private static string TX_BENEFICIARY_COUNTRY = "@id_beneficiaryCountry";
        private static string TX_BENEFICIARY_ACCIDENTIFIER = "@tx_beneficiaryAccIdentifier";
        private static string TX_BENEFICIARY_CURRENCY_CODE = "@tx_beneficiaryCurrencyCode";
        private static string DCC_BENEFICIARY_AMOUNT = "@dec_beneficiaryAmount";

        private static string ID_CUSTOMER_ID = "@id_customerId";
        private static string TX_CUSTOMER_NAME = "@tx_customerName";
        private static string TX_CUSTOMER_SURENAME = "@tx_customerSurname";
        private static string TX_CUSTOMER_GENDER = "@tx_customerGender";
        private static string TX_CUSTOMER_PASSPORT_NO = "@tx_customerPassportNo";
        private static string TX_CUSTOMER_PASSPORT_COUNTRY = "@id_customerPassportCountry";
        private static string TX_CUSTOMER_ADDRESS = "@tx_customerAddress";
        private static string TX_CUSTOMER_SUBURB = "@tx_customerSuburb";
        private static string TX_CUSTOMER_CITY = "@id_customerCity";
        private static string TX_CUSTOMER_ACCNO = "@tx_customerAccNo";
        private static string TX_CUSTOMER_PHONE = "@tx_customerPhone";

        private static string DCC_CUSTOMER_AMOUNT = "@dcc_customerAmount";
        private static string TX_REFERENCE_NO = "@tx_referenceNo";

        private static string ACT_ACCOUNT_KEY_SENDER = "@id_account_key";
        private static string ACT_ACCOUNT_KEY_RECEIVER = "@id_account_key";
        private static string ACT_DEC_BALANCE = "@dec_balance";

        private static string TNS_TRANSACTION_KEY = "@id_transaction_key";
        private static string TNS_TRANSACTION_CHARGE_KEY = "@id_transaction_charge_key";
        private static string TNS_SENDER_KEY = "@id_sender_key";
        private static string TNS_RECEIVER_KEY = "@id_receiver_key";
        private static string TNS_CURRENCY_KEY = "@id_currency_key";
        private static string TNS_TRANSFER_MSG = "@tx_transfer_msg";


        private static string TXN_TYPE_KEY = "@id_transaction_type_key";
        private static string TXN_AMT = "@dec_transaction_amount";
        private static string TXN_STATUS = "@tx_transaction_status";
        private static string TXN_EXT_TYPE_KEY = "@id_transaction_ext_type_key";

        private static string NTC_ID_TYPE_KEY = "@id_type_key";
        private static string NTC_ID_TO_KEY = "@id_to_key";
        private static string NTC_TX_NOTIFICATION = "@tx_notification";
        #endregion

        public RamittanceService(ICurrentUserService currentUserService, IDbConnection connection, ILogger<RamittanceService> logger)
        {
            _currentUserService = currentUserService;
            _connection = connection;
            _logger = logger;
        }
        public async Task<Result> InsertRamittanceInfo(Models.Ramittance request)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                _logger.LogInformation("request receive from remittance service");

                try
                {
                    string query = Constants.INS_RAMITTANCE;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(ID_REMITTANCE_KEY, request.IdRemittanceKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(ID_REMITTANCE_NO, request.IdRemittanceNo, DbType.Int32, ParameterDirection.Input);

                    parameter.Add(TX_BENEFICIARY_NAME, request.TxBeneficiaryName, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_BENEFICIARY_SURENAME, request.TxBeneficiarySurname, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_BENEFICIARY_COUNTRY, request.IdBeneficiaryCountry, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(TX_BENEFICIARY_ACCIDENTIFIER, request.TxBeneficiaryAccIdentifier, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_BENEFICIARY_CURRENCY_CODE, request.TxBeneficiaryCurrencyCode, DbType.String, ParameterDirection.Input);
                    parameter.Add(DCC_BENEFICIARY_AMOUNT, request.DecBeneficiaryAmount, DbType.Decimal, ParameterDirection.Input);

                    parameter.Add(ID_CUSTOMER_ID, request.IdCustomerId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_NAME, request.TxCustomerName, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_SURENAME, request.TxCustomerSurname, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_GENDER, request.TxCustomerGender, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_PASSPORT_NO, request.TxCustomerPassportNo, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_PASSPORT_COUNTRY, request.IdCustomerPassportCountry, DbType.Int32, ParameterDirection.Input);

                    parameter.Add(TX_CUSTOMER_ADDRESS, request.TxCustomerAddress, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_SUBURB, request.TxCustomerSuburb, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_CITY, request.IdCustomerCity, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_ACCNO, request.TxCustomerAccNo, DbType.String, ParameterDirection.Input);
                    parameter.Add(TX_CUSTOMER_PHONE, request.TxCustomerPhone, DbType.String, ParameterDirection.Input);
                    parameter.Add(DCC_CUSTOMER_AMOUNT, request.DccCustomerAmount, DbType.Decimal, ParameterDirection.Input);
                    parameter.Add(TX_REFERENCE_NO, request.TxReferenceNo, DbType.String, ParameterDirection.Input);

                    parameter.Add(Constants.ID_TRANSACTION_EXT_TYPE_KEY, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.TX_DESCRIPTION, request.TxDescription, DbType.String, ParameterDirection.Input);
                    parameter.Add(Constants.IS_ACTIVE, request.IsActive, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);

                    await _connection.ExecuteAsync(query, parameter, transaction: transaction);
                    int res = parameter.Get<int>(Constants.MESSAGE);

                    if (res > 0)
                    {
                        /*transaction table insert sp start*/
                        string queryForTransaction = Constants.ACT_TRANSACTION;
                        DynamicParameters parameterForTransaction = new DynamicParameters();
                        parameterForTransaction.Add(TXN_TYPE_KEY, 1, DbType.Int32, ParameterDirection.Input);
                        parameterForTransaction.Add(TXN_AMT, request.DccCustomerAmount, DbType.Decimal, ParameterDirection.Input);
                        parameterForTransaction.Add(TXN_STATUS, 'Y', DbType.String, ParameterDirection.Input);
                        parameterForTransaction.Add(TXN_EXT_TYPE_KEY, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);
                        parameterForTransaction.Add(Constants.TX_DESCRIPTION, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);
                        parameterForTransaction.Add(Constants.IS_ACTIVE, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);
                        parameterForTransaction.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
                        await _connection.ExecuteAsync(queryForTransaction, parameterForTransaction, transaction: transaction);
                        int resultForTransaction = parameter.Get<int>("@message");
                        /*transaction table insert sp end*/

                        /*account table insert sp start*/
                        string queryUpdateReciver = Constants.UPD_RECEIVER_ACCOUNT_BALANCE;
                        DynamicParameters parameterUpdateReceiver = new DynamicParameters();
                        parameterUpdateReceiver.Add(ACT_ACCOUNT_KEY_RECEIVER, 2, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateReceiver.Add(ACT_DEC_BALANCE, request.DccCustomerAmount, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateReceiver.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
                        await _connection.ExecuteAsync(queryUpdateReciver, parameterUpdateReceiver, transaction: transaction);
                        /*account table insert sp end*/

                        /*transafer table insert sp start*/
                        string spForInsertTransfer = Constants.INS_TRANSFER;
                        DynamicParameters parameterForInsertTransfer = new DynamicParameters();
                        parameterForInsertTransfer.Add(TNS_TRANSACTION_KEY, 3, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_TRANSACTION_CHARGE_KEY, 1, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_SENDER_KEY, 2, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_RECEIVER_KEY, 3, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_CURRENCY_KEY, 1, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_TRANSFER_MSG, "Test", DbType.String, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
                        await _connection.ExecuteAsync(spForInsertTransfer, parameterForInsertTransfer, transaction: transaction);
                        /*transafer table insert sp end*/

                        transaction.Commit();

                        /*notification start*/
                        //SendNotification();
                        /*notification end*/

                        _logger.LogInformation("commit done from remittance ");
                        return Result.Success("Remittance Sent Successfully");
                    }
                    else
                    {
                        _logger.LogError("Some thing wrong!!!");
                        transaction.Rollback();
                        _logger.LogInformation("roleback and something wrong ");
                        return Result.Failure(new List<string>() { "Something wrong" });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    transaction.Rollback();
                    _logger.LogInformation("exception from catch block : "+ ex.Message);
                    return Result.Failure(new List<string>() { "Error Msg:" + ex.Message });
                }
            }
        }

        public async void SendNotification()
        {
            _logger.LogInformation("remittance notification start");

            string spForInsertNotification = Constants.INS_NOTIFICATION;
            DynamicParameters parameterForInsertNotification = new DynamicParameters();

            parameterForInsertNotification.Add(NTC_ID_TYPE_KEY, 3, DbType.Int32, ParameterDirection.Input);
            parameterForInsertNotification.Add(NTC_ID_TO_KEY, 1, DbType.Int32, ParameterDirection.Input);
            parameterForInsertNotification.Add(NTC_TX_NOTIFICATION, "Remittance Send Successfully", DbType.String, ParameterDirection.Input);
            parameterForInsertNotification.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
            await _connection.ExecuteAsync(spForInsertNotification, parameterForInsertNotification);
            _logger.LogInformation("remittance notification end");

        }
    }
}
