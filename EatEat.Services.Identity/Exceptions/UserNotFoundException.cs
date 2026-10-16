namespace EatEat.Services.IdentityAPI.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public object UserId { get; }

        public UserNotFoundException(object userId)
            : base($"user with ID '{userId}' not found.")
        {
            UserId = userId;
        }
    }
}