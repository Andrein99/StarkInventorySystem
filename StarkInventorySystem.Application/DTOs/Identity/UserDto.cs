using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.Identity
{
    /// <summary>
    /// DTO para la información de perfil del usuario
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
