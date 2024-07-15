namespace SendMoneyService.Helpers
{
    public static class Constants
    {
        #region Common field
        public static string TX_DESCRIPTION = "@tx_description";
        public static string IS_ACTIVE = "@is_active";
        #endregion

        #region SP Name
        public static string ACT_TRANSACTION = "INS_transaction";
        public static string UPD_SENDER_ACCOUNT_BALANCE = "UPD_sender_account_balance";
        public static string UPD_RECEIVER_ACCOUNT_BALANCE = "UPD_receiver_account_balance";
        public static string INS_TRANSFER = "INS_transfer";
        #endregion
    }
}
