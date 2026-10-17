using System.ComponentModel.DataAnnotations;

namespace EatEat.Web.Models.Dto
{
    public class AssignRoleDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
