using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.Identity
{
    /// <summary>
    /// Request DTO para inicio de sesión del usuario
    /// </summary>
    public class LoginRequest
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
