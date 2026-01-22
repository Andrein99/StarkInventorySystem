using Microsoft.AspNetCore.Http;
using StarkInventorySystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public string? Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    ?.FindFirst(ClaimTypes.Email)?.Value;
            }
        }

        public string? Username
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    ?.FindFirst(ClaimTypes.Name)?.Value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public bool HasClaim(string claimType, string claimValue)
        {
            return _httpContextAccessor.HttpContext?.User
                ?.HasClaim(claimType, claimValue) ?? false;
        }
    }
}
