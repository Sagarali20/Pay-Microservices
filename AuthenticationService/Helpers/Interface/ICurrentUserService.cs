namespace AuthenticationService.Helpers.Interface
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        int RoleId { get; }
        public string UserName { get; }
        public string RoleName { get; }
    }
}
