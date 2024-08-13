namespace AuthenticationService.Models
{
    public class User
    {
       public int id_user_key { get; set; }
        public string tx_first_name { get; set; }
        public string tx_last_name { get; set; }
        public string tx_mobile_no { get; set; }         
        public string tx_password { get; set; }
        public string tx_email { get; set; }
        public List<Permission> Permission { get; set;}
    }
}
