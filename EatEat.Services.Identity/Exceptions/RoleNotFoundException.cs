namespace EatEat.Services.IdentityAPI.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public string RoleName { get; }

        public RoleNotFoundException(string roleName)
            : base($"role '{roleName}' not found.")
        {
            RoleName = roleName;
        }
    }
}