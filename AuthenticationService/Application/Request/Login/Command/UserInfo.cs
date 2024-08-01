namespace AuthenticationService.Application.Request.Login.Command
{
    public class UserInfo
    {
        public int IdUser { get; set; }
        public string TxFirstName { get; set; }
        public string TxLastName { get; set; }
        public string TxEmail { get; set; }
        public string TxMobileNo { get; set; }
        public string TxGender { get; set; }
        public string TxPassword { get; set; }
        public string TxIdentity { get; set; }
        public string TxDescription { get; set; }
        public DateTime DttDob { get; set; }
        public int IsActive { get; set; }
    }
}
