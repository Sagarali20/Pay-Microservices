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
        public static string Add_User_Document = "INS_user_document";
        public static string UPD_User_Document = "UPD_user_document";

        public static string UPD_User = "UPD_user";
        public static string UPD_Userpass = "UPD_Userpassword";

        public static string ADD_Login = "INS_login";
        public static string ADD_GenericMap = "INS_genericmap";
        public static string ADD_Group = "INS_group";
        public static string ADD_Role = "INS_role";
        public static string SEL_USERID = "SEL_GetPasswordByUserId";
        public static string CHECK_ID = "SEL_checkid";



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

    }
}
