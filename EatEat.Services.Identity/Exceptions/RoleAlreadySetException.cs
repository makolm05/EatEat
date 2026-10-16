namespace EatEat.Services.IdentityAPI.Exceptions
{
    public class RoleAlreadySetException : Exception
    {
        public object UserId { get; }
        public string RoleName { get; }

        public RoleAlreadySetException(object userId, string roleName)
            : base($"{userId} already have role {roleName}.")
        {
            UserId = userId;
            RoleName = roleName;
        }
    }
}
