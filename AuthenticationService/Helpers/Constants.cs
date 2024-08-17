namespace AuthenticationService.Helpers
{
    public static class Constants
    {
        #region Common field
        public static string TX_DESCRIPTION = "@tx_description";
        public static string IS_ACTIVE = "@is_active";
        #endregion

        #region SP Name
        public static string Add_User = "INS_user";
        public static string UPD_User = "UPD_user";
        public static string ADD_Login = "INS_login";
        public static string ADD_GenericMap = "INS_genericmap";
        public static string ADD_Group = "INS_group";
        public static string ADD_Role = "INS_role";

        #endregion

        #region T_User
        public static class Type
        {
            public const int User = 1;      
            public const int Group = 2;      
            public const int Role = 3;      
            public const int Permission = 4;      
        }
        #endregion

        public static void T_User()
        {
            Dictionary<string,int> dict = new Dictionary<string,int>();
            dict.Add("User", 1);
            dict.Add("Group", 2);
            dict.Add("Role", 3);
            dict.Add("Permission",4);

            //foreach (KeyValuePair<int, string> ele in My_dict)
            //{
            //    Console.WriteLine("{0} and {1}",
            //                ele.Key, ele.Value);
            //}

        }
        #region Common field
        public static string ID_TRANSACTION_EXT_TYPE_KEY = "@id_transaction_ext_type_key";
       // public static string TX_DESCRIPTION = "@tx_description";
       // public static string IS_ACTIVE = "@is_active";
        public static string MESSAGE = "@message";
        #endregion

        #region SP Name
        public static string ACT_TRANSACTION = "INS_transaction";
        public static string UPD_SENDER_ACCOUNT_BALANCE = "UPD_sender_account_balance";
        public static string UPD_RECEIVER_ACCOUNT_BALANCE = "UPD_receiver_account_balance";
        public static string INS_TRANSFER = "INS_transfer";
        public static string INS_RAMITTANCE = "INS_remittance";
        public static string INS_NOTIFICATION = "INS_notification";
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
