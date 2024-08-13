namespace AuthenticationService.Models
{
    public class Permission
    {
        public int id_permission_key { get; set; }    
        public int id_permission_type { get; set; }    
        public string tx_permission_name { get; set; }    
        public string tx_description { get; set; }
    }
}
