using AddMoneyService.Application.RequestMoney;
using AddMoneyService.Application.RequestMoney.Command;
using Common;
using Common.Interface;
using Dapper;
using System.Data;
using System.Transactions;

namespace AddMoneyService.Service
{
    public class RequestMoneyService : IRequestMoneyService
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<RequestMoneyService> _logger;
        private readonly ICurrentUserService _currentUserService;

        #region stored procedure column name
        private static string tx_sender_phone = "@tx_sender_phone";
        private static string tx_receiver_phone = "@tx_receiver_phone";
        private static string dcc_amount = "@dcc_amount";
        private static string dcc_charge_amount = "@dcc_charge_amount";
        private static string tx_reference_note = "@tx_reference_note";
        private static string is_success = "@is_success";

        private static string NTC_ID_TYPE_KEY = "@id_type_key";
        private static string NTC_ID_TO_KEY = "@id_to_key";
        private static string NTC_TX_NOTIFICATION = "@tx_notification";
        #endregion stored procedure column name

        public RequestMoneyService(IDbConnection connection, ILogger<RequestMoneyService> logger, ICurrentUserService currentUserService)
        {
            _connection = connection;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> AddRequestMoeny(AddRequestMoney request)
        {
            _logger.LogInformation("request receive from service");
            int result = 0;
            foreach (var item in request.AddMoney)
            {

                if (item.Amount < 0)
                {
                    _logger.LogInformation("user input negative amount : " + item.Amount);
                    return Result.Failure(new List<string> { "Invalid Amount" });
                }

                _logger.LogInformation("requst received from this sender no: " + item.SenderPhone + " amount :" + item.Amount + " receiver contact no : " + item.ReceiverPhone + " Charge Amount : " + item.ChargeAmount);


                string query = Constants.INS_REQUEST_MONEY;
                DynamicParameters parameter = new DynamicParameters();

                parameter.Add(Constants.ID_USER_KEY, _currentUserService.UserId, DbType.Int32, ParameterDirection.Input);
                parameter.Add(tx_sender_phone, item.SenderPhone, DbType.String, ParameterDirection.Input);

                parameter.Add(tx_receiver_phone, item.ReceiverPhone, DbType.String, ParameterDirection.Input);
                parameter.Add(dcc_amount, item.Amount, DbType.Decimal, ParameterDirection.Input);
                parameter.Add(dcc_charge_amount, item.ChargeAmount, DbType.Decimal, ParameterDirection.Input);
                parameter.Add(tx_reference_note, item.ReferenceNote, DbType.String, ParameterDirection.Input);
                parameter.Add(is_success, false, DbType.Decimal, ParameterDirection.Input);

                parameter.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
                await _connection.ExecuteAsync(query, parameter);

                result = parameter.Get<int>(Constants.MESSAGE);
            }
            if(result > 0)
            {
                _logger.LogInformation("request money success");
                return Result.Success("success");
            }
            else
            {
                _logger.LogInformation("request money failed");
                return Result.Failure(new List<string> { "Something wrong! try again latter" });
            }
        }

        public async void SendNotify()
        {
            _logger.LogInformation("notification receive");

            string spForInsertNotification = Constants.INS_NOTIFICATION;
            DynamicParameters parameterForInsertNotification = new DynamicParameters();

            parameterForInsertNotification.Add(NTC_ID_TYPE_KEY, 3, DbType.Int32, ParameterDirection.Input);
            parameterForInsertNotification.Add(NTC_ID_TO_KEY, 1, DbType.Int32, ParameterDirection.Input);
            parameterForInsertNotification.Add(NTC_TX_NOTIFICATION, "Request Money Successfully", DbType.String, ParameterDirection.Input);
            parameterForInsertNotification.Add(Constants.MESSAGE, "", DbType.Int32, ParameterDirection.Output);
            await _connection.ExecuteAsync(spForInsertNotification, parameterForInsertNotification);
            _logger.LogInformation("notification done");
        }

    }
}
