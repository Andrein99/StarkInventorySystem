using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.Identity
{
    /// <summary>
    /// Response DTO que contiene la información del usuario y el token JWT.
    /// </summary>
    public class AuthenticationResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }

    }
}
