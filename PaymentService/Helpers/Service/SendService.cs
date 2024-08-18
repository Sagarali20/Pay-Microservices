using Common;
using Common.Interface;
using Dapper;
using PaymentService.Application.Request.Send;
using PaymentService.Application.Request.Send.Command;
using System.Data;
using System.Transactions;

namespace PaymentService.Helpers.Service
{
    public class SendService : ISendService
    {
        private readonly DapperContext _dapperContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SendService> _logger;
        #region sp_parameter
        private static string TXN_TYPE_KEY = "@id_transaction_type_key";
        private static string TXN_AMT = "@dec_transaction_amount";
        private static string TXN_STATUS = "@tx_transaction_status";
        private static string TXN_EXT_TYPE_KEY = "@id_transaction_ext_type_key";

        private static string ACT_ACCOUNT_KEY_SENDER = "@id_account_key";
        private static string ACT_ACCOUNT_KEY_RECEIVER = "@id_account_key";
        private static string ACT_DEC_BALANCE = "@dec_balance";

        private static string TNS_TRANSACTION_KEY = "@id_transaction_key";
        private static string TNS_TRANSACTION_CHARGE_KEY = "@id_transaction_charge_key";
        private static string TNS_SENDER_KEY = "@id_sender_key";
        private static string TNS_RECEIVER_KEY = "@id_receiver_key";
        private static string TNS_CURRENCY_KEY = "@id_currency_key";
        private static string TNS_TRANSFER_MSG = "@tx_transfer_msg";

        private static string NTC_ID_TYPE_KEY = "@id_type_key";
        private static string NTC_ID_TO_KEY = "@id_to_key";
        private static string NTC_TX_NOTIFICATION = "@tx_notification";

        private static string Message = "@message";
        #endregion
        public SendService(DapperContext dapperContext, ICurrentUserService currentUserService, ILogger<SendService> logger)
        {
            _dapperContext = dapperContext;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<Result> SendMoney(AddOrEditTransaction request)
        {
            _logger.LogInformation("Request receive from service");
            using (var context = _dapperContext.CreateConnection())
            {
                _logger.LogInformation("Request receive from dapper context connection");
                context.Open();
                _logger.LogInformation("connection open");
                using (var transactionScope = context.BeginTransaction())
                {
                    _logger.LogInformation("transaction open");

                    string qryForCheckAvailableBalance = "select dec_balance from T_ACCOUNT where id_account_key =" + 2;
                    var data = context.ExecuteScalar(qryForCheckAvailableBalance, transaction: transactionScope);
                    if (!(Convert.ToInt32(data) >= request.DecTransactionAmount))
                    {
                        _logger.LogInformation("insufficient Balance");
                        return Result.Failure(new List<string> { "insufficient Balance" });
                    }

                    string query = Constants.ACT_TRANSACTION;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(TXN_TYPE_KEY, request.IdTransactionTypeKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(TXN_AMT, request.DecTransactionAmount, DbType.Decimal, ParameterDirection.Input);
                    parameter.Add(TXN_STATUS, request.TxTransactionStatus, DbType.String, ParameterDirection.Input);
                    parameter.Add(TXN_EXT_TYPE_KEY, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);

                    parameter.Add(Constants.TX_DESCRIPTION, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.IS_ACTIVE, request.IdTransactionExtTypeKey, DbType.Int32, ParameterDirection.Input);

                    parameter.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                    await context.ExecuteAsync(query, parameter, transaction: transactionScope);
                    int res = parameter.Get<int>(Message);


                    if (res > 0)
                    {
                        string queryUpdateSender = Constants.UPD_SENDER_ACCOUNT_BALANCE;
                        DynamicParameters parameterUpdateSender = new DynamicParameters();
                        parameterUpdateSender.Add(ACT_ACCOUNT_KEY_SENDER, 2, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateSender.Add(ACT_DEC_BALANCE, request.DecTransactionAmount, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateSender.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(queryUpdateSender, parameterUpdateSender, transaction: transactionScope);


                        string queryUpdateReciver = Constants.UPD_RECEIVER_ACCOUNT_BALANCE;
                        DynamicParameters parameterUpdateReceiver = new DynamicParameters();

                        parameterUpdateReceiver.Add(ACT_ACCOUNT_KEY_RECEIVER, 3, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateReceiver.Add(ACT_DEC_BALANCE, request.DecTransactionAmount, DbType.Int32, ParameterDirection.Input);
                        parameterUpdateReceiver.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(queryUpdateReciver, parameterUpdateReceiver, transaction: transactionScope);

                        string spForInsertTransfer = Constants.INS_TRANSFER;
                        DynamicParameters parameterForInsertTransfer = new DynamicParameters();

                        parameterForInsertTransfer.Add(TNS_TRANSACTION_KEY, 3, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_TRANSACTION_CHARGE_KEY, 1, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_SENDER_KEY, 2, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_RECEIVER_KEY, 3, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_CURRENCY_KEY, 1, DbType.Int32, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(TNS_TRANSFER_MSG, "Test", DbType.String, ParameterDirection.Input);
                        parameterForInsertTransfer.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(spForInsertTransfer, parameterForInsertTransfer, transaction: transactionScope);

                        SendNotify();

                        _logger.LogInformation("transaction success");
                        transactionScope.Commit();
                        return Result.Success("send money successfully");

                    }
                    else
                    {
                        transactionScope.Rollback();
                        _logger.LogInformation("transaction failed");
                        return Result.Failure(new List<string>() { "Something wrong" });
                    }
                }
            }
        }

        public async void SendNotify()
        {
            _logger.LogInformation("Request receive from service");
            using (var context = _dapperContext.CreateConnection())
            {
                string spForInsertNotification = Constants.INS_NOTIFICATION;
                DynamicParameters parameterForInsertNotification = new DynamicParameters();

                parameterForInsertNotification.Add(NTC_ID_TYPE_KEY, 3, DbType.Int32, ParameterDirection.Input);
                parameterForInsertNotification.Add(NTC_ID_TO_KEY, 1, DbType.Int32, ParameterDirection.Input);
                parameterForInsertNotification.Add(NTC_TX_NOTIFICATION, "Send Money Successfully", DbType.String, ParameterDirection.Input);
                parameterForInsertNotification.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                await context.ExecuteAsync(spForInsertNotification, parameterForInsertNotification);
                _logger.LogInformation("notification success");
            }
        }

        public async Task<Result> SendNotification(SendNotification request)
        {
            _logger.LogInformation("Request receive from service");
            using (var context = _dapperContext.CreateConnection())
            {
                string spForInsertNotification = Constants.INS_NOTIFICATION;
                DynamicParameters parameterForInsertNotification = new DynamicParameters();

                parameterForInsertNotification.Add(NTC_ID_TYPE_KEY, 3, DbType.Int32, ParameterDirection.Input);
                parameterForInsertNotification.Add(NTC_ID_TO_KEY, 1, DbType.Int32, ParameterDirection.Input);
                parameterForInsertNotification.Add(NTC_TX_NOTIFICATION, "Send Money Successfully", DbType.String, ParameterDirection.Input);
                parameterForInsertNotification.Add(Message, "", DbType.Int32, ParameterDirection.Output);
                await context.ExecuteAsync(spForInsertNotification, parameterForInsertNotification);

                _logger.LogInformation("notification success");
                return Result.Success("notification success");
            }
        }
    }
}
