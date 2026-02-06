using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.Identity
{
    /// <summary>
    /// Request DTO para el registro de usuarios
    /// </summary>
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
