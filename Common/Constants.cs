namespace Common
{
    public static class Constants
    {
        #region Common field
        public static string ID_TRANSACTION_EXT_TYPE_KEY = "@id_transaction_ext_type_key";
        public static string TX_DESCRIPTION = "@tx_description";
        public static string IS_ACTIVE = "@is_active";
        public static string MESSAGE = "@message";
        #endregion

        #region SP Name
        public static string ACT_TRANSACTION = "INS_transaction";
        public static string UPD_SENDER_ACCOUNT_BALANCE = "UPD_sender_account_balance";
        public static string UPD_RECEIVER_ACCOUNT_BALANCE = "UPD_receiver_account_balance";
        public static string INS_TRANSFER = "INS_transfer";
        public static string INS_RAMITTANCE = "INS_remittance";
        public static string INS_NOTIFICATION = "INS_notification";
        public static string INS_REQUEST_MONEY = "INS_request_money";
        #endregion

        #region notification message
        public static string NOTIFICATION_SEND_MONEY = "Send Money Successfully";
        public static string NOTIFICATION_REMITTANCE = "Remittance Send Successfully";
        #endregion

        #region success and fail message
        public static string SUCCESS = "Success";
        public static string FAIL = "Failed";
        #endregion
    }
}
